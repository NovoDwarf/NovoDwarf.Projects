using System.Reflection;
using System.Xml.Linq;
using Aegis.Packaging.Definitions.Entities;
using Aegis.Packaging.Definitions.Reader.Entities;
using Aegis.Packaging.Definitions.Reader.Interfaces;
using Aegis.Packaging.Definitions.Reader.Parsers;
using Aegis.Packaging.Definitions.Reader.Strategies;
using Grekov;

namespace Aegis.Packaging.Definitions.Reader.Services;

internal sealed class DefinitionXmlReader
{
	private static readonly NullabilityInfoContext NullabilityInfoContext = new();

	private static readonly MethodInfo? PackageIdSetter = typeof(Def)
		.GetProperty(nameof(Def.PackageId), BindingFlags.Instance | BindingFlags.Public)?
		.GetSetMethod(nonPublic: true);

	private static readonly MethodInfo? ResourcePathSetter = typeof(Def)
		.GetProperty(nameof(Def.ResourcePath), BindingFlags.Instance | BindingFlags.Public)?
		.GetSetMethod(nonPublic: true);

	private static readonly IFieldValueStrategy[] FieldValueStrategies =
	[
		new CollectionFieldValueStrategy(),
		new ScalarFieldValueStrategy()
	];

	private static readonly IXmlValueStrategy[] ValueStrategies =
	[
		new XmlScalarValueStrategy(),
		new XmlAssetValueStrategy(),
		new XmlReferenceValueStrategy(),
		new XmlObjectValueStrategy()
	];

	private Dictionary<string, Type> _namedTypes;
	private readonly Dictionary<Type, TypeMetadata> _typeMetadataCache = new();
	private readonly IFileSystemService _fileSystem;

	public DefinitionXmlReader(IFileSystemService fileSystem)
	{
		_fileSystem = fileSystem;
		_namedTypes = BuildNamedTypeMap();
	}

	public void RefreshTypeMap()
	{
		_namedTypes = BuildNamedTypeMap();
	}

	public Def ReadDefinition(string packageId, string resourcePath, string fallbackId,
		List<PendingReference> pendingReferences)
	{
		var text = _fileSystem.ReadAllText(resourcePath);

		if (string.IsNullOrWhiteSpace(text))
			throw new InvalidOperationException($"Definition xml is empty: '{resourcePath}'.");

		var root = XDocument.Parse(text).Root ??
		           throw new InvalidOperationException($"Definition xml has no root: '{resourcePath}'.");

		var rootType = ResolveType(root.Name.LocalName, typeof(Def), resourcePath, root);
		var result = ParseObject(
			targetType: rootType,
			element: root,
			packageId: packageId,
			resourcePath: resourcePath,
			fallbackId: fallbackId,
			pendingReferences: pendingReferences);

		return result as Def ?? throw new InvalidOperationException(
			$"Definition root element '{root.Name.LocalName}' in '{resourcePath}' was parsed as '{result.GetType().FullName}', which is not a Def.");
	}

	internal object ParseObject(
		Type targetType,
		XElement element,
		string packageId,
		string resourcePath,
		string? fallbackId,
		List<PendingReference> pendingReferences)
	{
		targetType = ResolveElementType(element, targetType, resourcePath);

		var metadata = GetTypeMetadata(targetType);
		var instance = metadata.Factory() ?? throw new InvalidOperationException(
			$"Failed to instantiate '{targetType.FullName}' for element '{element.Name.LocalName}' in '{resourcePath}'.");

		InitializeInstance(instance, element, packageId, resourcePath, fallbackId);
		PopulateFields(instance, metadata, element, packageId, resourcePath, pendingReferences);

		return instance;
	}

	internal FieldReadResult ReadValue(XmlValueReadContext context)
	{
		foreach (var strategy in ValueStrategies)
		{
			if (!strategy.CanHandle(context))
				continue;

			var result = strategy.Read(context);
			if (result.Status != FieldReadStatus.Missing)
				return result;
		}

		return FieldReadResult.Missing();
	}

	private void InitializeInstance(object instance, XElement element, string packageId, string resourcePath,
		string? fallbackId)
	{
		if (instance is not Def def)
			return;

		AssignDefOrigin(def, packageId, resourcePath);

		var rawId = DefinitionXmlValueParsers.GetAttribute(element, "id");

		if (!string.IsNullOrWhiteSpace(rawId) && !DefId.TryParse(rawId, out _))
			throw new FormatException(
				$"Definition id '{rawId}' is invalid for resource [{resourcePath}] in package [{packageId}].");

		if (DefId.TryParse(rawId, out var id) || DefId.TryParse(fallbackId, out id))
		{
			def.Id = id;
			return;
		}

		if (fallbackId == null)
			return;

		throw new FormatException(
			$"Definition id is missing or invalid for resource [{resourcePath}] in package [{packageId}].");
	}

	private void PopulateFields(
		object instance,
		TypeMetadata metadata,
		XElement element,
		string packageId,
		string resourcePath,
		List<PendingReference> pendingReferences)
	{
		foreach (var field in metadata.Fields)
		{
			var result = ReadFieldValue(
				ownerInstance: instance,
				fieldMetadata: field,
				ownerElement: element,
				packageId: packageId,
				resourcePath: resourcePath,
				pendingReferences: pendingReferences);

			switch (result.Status)
			{
				case FieldReadStatus.Success:
					if (!ReferenceEquals(result.Value, PendingValue.Instance))
						field.Setter(instance, result.Value);
					break;

				case FieldReadStatus.Missing:
					if (field.Attribute.Required)
					{
						throw new InvalidOperationException(
							BuildFieldErrorMessage(
								resourcePath,
								metadata.Type.Name,
								field.Property.Name,
								field.FieldName,
								element.Name.LocalName,
								$"Required XML field [{field.FieldName}] is missing."));
					}

					ApplyMissingTextureFallback(instance, field);
					break;

				case FieldReadStatus.Invalid:
					throw new InvalidOperationException(
						BuildFieldErrorMessage(
							resourcePath,
							metadata.Type.Name,
							field.Property.Name,
							field.FieldName,
							element.Name.LocalName,
							result.ErrorMessage ?? "Invalid XML value."));
			}
		}
	}

	private static void ApplyMissingTextureFallback(object instance, FieldMetadata field)
	{
		if (!ShouldUseTexturePlaceholder(field.Property))
			return;

		if (field.Property.GetValue(instance) != null)
			return;

		field.Setter(instance, TextureUtils.Placeholder);
	}

	private static bool ShouldUseTexturePlaceholder(PropertyInfo property)
	{
		if (property.PropertyType != typeof(Texture2D))
			return false;

		var nullability = NullabilityInfoContext.Create(property);
		return nullability.WriteState != NullabilityState.Nullable;
	}

	private FieldReadResult ReadFieldValue(
		object ownerInstance,
		FieldMetadata fieldMetadata,
		XElement ownerElement,
		string packageId,
		string resourcePath,
		List<PendingReference> pendingReferences)
	{
		var field = fieldMetadata.Attribute;
		var fieldName = fieldMetadata.FieldName;

		var attrValue = field.Kind is DefFieldKind.Auto or DefFieldKind.Attribute
			? DefinitionXmlValueParsers.GetAttribute(ownerElement, fieldName)
			: null;

		if (!string.IsNullOrWhiteSpace(attrValue))
		{
			try
			{
				return ReadFieldValueWithStrategies(new FieldValueReadContext(
					this,
					ownerInstance,
					fieldMetadata,
					ownerElement,
					packageId,
					resourcePath,
					pendingReferences,
					FieldValueSource.Attribute,
					null,
					attrValue));
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(
					BuildFieldErrorMessage(
						resourcePath,
						ownerInstance.GetType().Name,
						fieldMetadata.Property.Name,
						fieldName,
						$"@{fieldName}",
						ex.Message),
					ex);
			}
		}

		if (field.Kind == DefFieldKind.Attribute)
			return FieldReadResult.Missing();

		var child = field.Kind == DefFieldKind.Reference &&
		            string.Equals(fieldName, "ref", StringComparison.OrdinalIgnoreCase)
			? ownerElement
			: DefinitionXmlValueParsers.GetChild(ownerElement, fieldName);

		if (child == null)
			return FieldReadResult.Missing();

		try
		{
			return ReadFieldValueWithStrategies(new FieldValueReadContext(
				this,
				ownerInstance,
				fieldMetadata,
				ownerElement,
				packageId,
				resourcePath,
				pendingReferences,
				FieldValueSource.Element,
				child,
				null));
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(
				BuildFieldErrorMessage(
					resourcePath,
					ownerInstance.GetType().Name,
					fieldMetadata.Property.Name,
					fieldName,
					child.Name.LocalName,
					ex.Message),
				ex);
		}
	}

	private static FieldReadResult ReadFieldValueWithStrategies(FieldValueReadContext context)
	{
		foreach (var strategy in FieldValueStrategies)
		{
			if (!strategy.CanHandle(context))
				continue;

			var result = strategy.Read(context);
			if (result.Status != FieldReadStatus.Missing)
				return result;
		}

		return FieldReadResult.Missing();
	}

	private Type ResolveElementType(XElement element, Type fallbackType, string resourcePath)
	{
		return fallbackType is { IsAbstract: false, IsInterface: false }
			? fallbackType
			: ResolveType(element.Name.LocalName, fallbackType, resourcePath, element);
	}

	private Type ResolveType(string name, Type expectedBaseType, string resourcePath, XElement element)
	{
		if (!_namedTypes.TryGetValue(name, out var type))
		{
			throw new InvalidOperationException(
				$"Unknown definition xml type '{name}' in '{resourcePath}' at element '{element.Name.LocalName}'.");
		}

		if (!expectedBaseType.IsAssignableFrom(type))
		{
			throw new InvalidOperationException(
				$"Xml type '{name}' resolved to '{type.FullName}', but it is not assignable to '{expectedBaseType.FullName}' " +
				$"in '{resourcePath}' at element '{element.Name.LocalName}'.");
		}

		return type;
	}

	private TypeMetadata GetTypeMetadata(Type type)
	{
		if (_typeMetadataCache.TryGetValue(type, out var cached))
			return cached;

		var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

		var fields = properties
			.Where(static property => property.CanWrite && property.GetIndexParameters().Length == 0)
			.Where(static property => property.Name is not nameof(Def.Id)
			                          && property.Name is not nameof(Def.PackageId)
			                          && property.Name is not nameof(Def.ResourcePath)
			                          && property.Name is not nameof(Def.ResourceName))
			.Select(property =>
			{
				var attr = property.GetCustomAttribute<DefFieldAttribute>(inherit: true)
				           ?? GetImplicitComponentFieldAttribute(type, property);
				return attr == null ? null : BuildFieldMetadata(property, attr);
			})
			.Where(static field => field != null)
			.Cast<FieldMetadata>()
			.ToArray();

		var metadata = new TypeMetadata(
			Type: type,
			Factory: BuildFactory(type),
			Fields: fields);

		_typeMetadataCache[type] = metadata;
		return metadata;
	}

	private static FieldMetadata BuildFieldMetadata(PropertyInfo property, DefFieldAttribute attr)
	{
		var setter = property.SetMethod ??
		             throw new InvalidOperationException($"Writable property '{property.Name}' has no set method.");

		return new FieldMetadata(
			Property: property,
			PropertyType: property.PropertyType,
			Attribute: attr,
			FieldName: GetFieldName(property, attr),
			Setter: (target, value) => setter.Invoke(target, [value]));
	}

	private static Func<object> BuildFactory(Type type)
	{
		return () => Activator.CreateInstance(type)
		             ?? throw new InvalidOperationException($"Failed to instantiate '{type.FullName}'.");
	}

	private static void AssignDefOrigin(Def def, string packageId, string resourcePath)
	{
		PackageIdSetter?.Invoke(def, [packageId]);
		ResourcePathSetter?.Invoke(def, [resourcePath]);
	}

	private static Dictionary<string, Type> BuildNamedTypeMap()
	{
		var allTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(static assembly =>
			{
				try
				{
					return assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException ex)
				{
					return ex.Types.Where(static type => type != null)!;
				}
			})
			.Where(static type =>
				type is { IsAbstract: false } &&
				(typeof(Def).IsAssignableFrom(type) || typeof(ComponentDef).IsAssignableFrom(type)))
			.ToArray();

		var aliasGroups = allTypes
			.SelectMany(static type => GetTypeAliases(type).Select(alias => new { Alias = alias, Type = type }))
			.GroupBy(static x => x.Alias, StringComparer.OrdinalIgnoreCase)
			.ToArray();

		var conflicts = aliasGroups
			.Where(group => group.Select(x => x.Type).Distinct().Count() > 1)
			.ToArray();

		if (conflicts.Length > 0)
		{
			var lines = conflicts.Select(group =>
				$"Alias '{group.Key}' is declared by: {string.Join(", ", group.Select(x => x.Type.FullName).Distinct())}");

			throw new InvalidOperationException(
				"Duplicate xml type aliases detected:" + System.Environment.NewLine +
				string.Join(System.Environment.NewLine, lines));
		}

		return aliasGroups.ToDictionary(
			static group => group.Key,
			static group => group.First().Type,
			StringComparer.OrdinalIgnoreCase);
	}

	private static IEnumerable<string> GetTypeAliases(Type type)
	{
		var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			type.Name
		};

		if (type.Name.EndsWith("Def", StringComparison.Ordinal) && type.Name.Length > "Def".Length)
			aliases.Add(type.Name[..^"Def".Length]);

		var alias = type.GetCustomAttribute<DefTypeAttribute>(inherit: false)?.ElementName;

		if (!string.IsNullOrWhiteSpace(alias))
			aliases.Add(alias);

		foreach (var item in aliases)
			yield return item;
	}

	private static string GetFieldName(PropertyInfo property, DefFieldAttribute field)
	{
		return string.IsNullOrWhiteSpace(field.Name) ? property.Name : field.Name;
	}

	private static DefFieldAttribute? GetImplicitComponentFieldAttribute(Type ownerType, PropertyInfo property)
	{
		if (!typeof(ComponentDef).IsAssignableFrom(ownerType))
			return null;

		return property.GetCustomAttribute<ExportAttribute>(inherit: true) != null
			? new DefFieldAttribute()
			: null;
	}

	private static string BuildFieldErrorMessage(
		string resourcePath,
		string typeName,
		string propertyName,
		string fieldName,
		string xmlLocation,
		string details)
	{
		return
			$"Definition XML read failed.\n" +
			$"File: {resourcePath}\n" +
			$"Type: {typeName}\n" +
			$"Property: {propertyName}\n" +
			$"Field: {fieldName}\n" +
			$"Xml: {xmlLocation}\n" +
			$"Details: {details}";
	}
}