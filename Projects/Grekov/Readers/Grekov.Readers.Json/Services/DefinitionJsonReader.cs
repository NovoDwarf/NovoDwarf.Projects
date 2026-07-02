using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Nodes;
using Grekov.Core;
using Grekov.Core.Attributes;
using Grekov.Core.Enums;
using Grekov.Definitions.Entities;
using Grekov.Definitions.Interfaces;
using NovoDwarf.FS.Interfaces;

namespace Grekov.Readers.Json.Services;

public sealed class DefinitionJsonReader : IDefinitionFormatReader
{
	private static readonly IReadOnlySet<string> SupportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		".json"
	};

	private static readonly string[] TypeProperties = ["$type", "type", "defType"];
	private static readonly string[] ContainerProperties = ["defs", "definitions", "localization", "localizations", "translations"];

	private static readonly MethodInfo? PackageIdSetter = typeof(Def)
		.GetProperty(nameof(Def.PackageId), BindingFlags.Instance | BindingFlags.Public)?
		.GetSetMethod(nonPublic: true);

	private static readonly MethodInfo? ResourcePathSetter = typeof(Def)
		.GetProperty(nameof(Def.ResourcePath), BindingFlags.Instance | BindingFlags.Public)?
		.GetSetMethod(nonPublic: true);

	private readonly IFileSystemService _fileSystem;
	private readonly Dictionary<Type, DefTypeMetadata> _metadataByType = [];
	private Dictionary<string, Type> _namedTypes;

	public DefinitionJsonReader(IFileSystemService fileSystem)
	{
		_fileSystem = fileSystem;
		_namedTypes = BuildNamedTypeMap();
	}

	public IReadOnlySet<string> Extensions => SupportedExtensions;

	public void RefreshTypeMap()
	{
		_namedTypes = BuildNamedTypeMap();
	}

	public IReadOnlyList<Def> ReadDefinitions(DefinitionReadContext context)
	{
		var text = _fileSystem.ReadAllText(context.ResourcePath);
		if (string.IsNullOrWhiteSpace(text))
			throw new InvalidOperationException($"Definition json is empty: '{context.ResourcePath}'.");

		var root = JsonNode.Parse(text) ??
		           throw new InvalidOperationException($"Definition json has no root: '{context.ResourcePath}'.");

		var result = new List<Def>();
		ReadDefinitions(context, root, result);
		return result;
	}

	private void ReadDefinitions(DefinitionReadContext context, JsonNode root, List<Def> result)
	{
		switch (root)
		{
			case JsonArray array:
				ReadArray(context, array, result);
				return;

			case JsonObject obj:
				if (TryGetContainerArray(obj, out var container))
				{
					ReadArray(context, container, result);
					return;
				}

				if (TryUnwrapTypedObject(obj, out var typeName, out var wrapped))
				{
					var type = ResolveType(typeName, typeof(Def), context.ResourcePath);
					result.Add(ReadDefinition(context, wrapped, type, context.FallbackId));
					return;
				}

				var explicitType = GetTypeName(obj);
				if (!string.IsNullOrWhiteSpace(explicitType))
				{
					var type = ResolveType(explicitType, typeof(Def), context.ResourcePath);
					result.Add(ReadDefinition(context, obj, type, context.FallbackId));
					return;
				}

				throw new InvalidOperationException($"Definition json object in '{context.ResourcePath}' has no type marker.");

			default:
				throw new InvalidOperationException($"Definition json root in '{context.ResourcePath}' must be an object or an array.");
		}
	}

	private void ReadArray(DefinitionReadContext context, JsonArray array, List<Def> result)
	{
		var index = 0;
		foreach (var item in array)
		{
			if (item is not JsonObject obj)
				throw new InvalidOperationException($"Definition json array item #{index} in '{context.ResourcePath}' is not an object.");

			var fallbackId = $"{context.FallbackId}/{index}";

			if (TryUnwrapTypedObject(obj, out var wrapperTypeName, out var wrapped))
			{
				var wrapperType = ResolveType(wrapperTypeName, typeof(Def), context.ResourcePath);
				result.Add(ReadDefinition(context, wrapped, wrapperType, fallbackId));
			}
			else
			{
				var typeName = GetTypeName(obj) ??
				               throw new InvalidOperationException($"Definition json array item #{index} in '{context.ResourcePath}' has no type marker.");
				var type = ResolveType(typeName, typeof(Def), context.ResourcePath);
				result.Add(ReadDefinition(context, obj, type, fallbackId));
			}

			index++;
		}
	}

	private Def ReadDefinition(DefinitionReadContext context, JsonObject obj, Type type, string fallbackId)
	{
		var result = ParseObject(type, obj, context.PackageId, context.ResourcePath, fallbackId, context.PendingReferences);

		return result as Def ?? throw new InvalidOperationException(
			$"Definition json object in '{context.ResourcePath}' was parsed as '{result.GetType().FullName}', which is not a Def.");
	}

	private object ParseObject(
		Type targetType,
		JsonObject obj,
		string packageId,
		string resourcePath,
		string? fallbackId,
		List<DefPendingReference> pendingReferences)
	{
		targetType = ResolveObjectType(obj, targetType, resourcePath);

		var metadata = GetTypeMetadata(targetType);
		var instance = metadata.Factory() ?? throw new InvalidOperationException(
			$"Failed to instantiate '{targetType.FullName}' in '{resourcePath}'.");

		InitializeInstance(instance, obj, packageId, resourcePath, fallbackId);
		PopulateFields(instance, metadata, obj, packageId, resourcePath, pendingReferences);

		return instance;
	}

	private void InitializeInstance(object instance, JsonObject obj, string packageId, string resourcePath, string? fallbackId)
	{
		if (instance is not Def def)
			return;

		PackageIdSetter?.Invoke(def, [packageId]);
		ResourcePathSetter?.Invoke(def, [resourcePath]);

		var rawId = GetScalar(GetProperty(obj, "id"));

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
		DefTypeMetadata metadata,
		JsonObject obj,
		string packageId,
		string resourcePath,
		List<DefPendingReference> pendingReferences)
	{
		foreach (var field in metadata.Fields)
		{
			var node = field.Attribute.Kind == DefFieldKind.Reference &&
			           string.Equals(field.FieldName, "ref", StringComparison.OrdinalIgnoreCase)
				? obj
				: GetProperty(obj, field.FieldName);

			if (node == null)
			{
				if (field.Attribute.Required)
					throw new InvalidOperationException($"Required JSON field [{field.FieldName}] is missing in '{resourcePath}'.");

				continue;
			}

			var value = ConvertValue(node, field.PropertyType, instance, field, packageId, resourcePath, pendingReferences);

			if (!ReferenceEquals(value, PendingValue.Instance))
				field.Setter(instance, value);
		}
	}

	private object? ConvertValue(
		JsonNode node,
		Type targetType,
		object ownerInstance,
		FieldMetadata field,
		string packageId,
		string resourcePath,
		List<DefPendingReference> pendingReferences)
	{
		if (field.Attribute.Kind == DefFieldKind.Collection || node is JsonArray || TryGetCollectionItemType(targetType, out _))
			return ReadCollection(node, targetType, field, packageId, resourcePath, pendingReferences);

		if (field.Attribute.Kind == DefFieldKind.Reference || typeof(Def).IsAssignableFrom(targetType))
			return ReadReferenceOrObject(node, targetType, ownerInstance, field, packageId, resourcePath, pendingReferences);

		if (node is JsonObject obj && targetType is { IsAbstract: false, IsInterface: false } && !IsScalarType(targetType))
			return ParseObject(targetType, obj, packageId, resourcePath, null, pendingReferences);

		return ReadScalar(GetScalar(node), targetType, field.FieldName, ownerInstance.GetType().Name, resourcePath);
	}

	private object? ReadReferenceOrObject(
		JsonNode node,
		Type targetType,
		object ownerInstance,
		FieldMetadata field,
		string packageId,
		string resourcePath,
		List<DefPendingReference> pendingReferences)
	{
		var referenceIdRaw = node is JsonObject obj
			? GetScalar(GetProperty(obj, "ref"))
			: GetScalar(node);

		if (!string.IsNullOrWhiteSpace(referenceIdRaw))
		{
			var referenceId = DefId.Parse(referenceIdRaw);
			pendingReferences.Add(new DefPendingReference(
				targetType,
				referenceId.ToString(),
				resourcePath,
				value => field.Setter(ownerInstance, value)));

			return PendingValue.Instance;
		}

		if (node is not JsonObject nested)
			throw new InvalidOperationException($"Reference/object field [{field.FieldName}] in '{resourcePath}' must be an object or ref string.");

		return ParseObject(targetType, nested, packageId, resourcePath, null, pendingReferences);
	}

	private object ReadCollection(
		JsonNode node,
		Type propertyType,
		FieldMetadata field,
		string packageId,
		string resourcePath,
		List<DefPendingReference> pendingReferences)
	{
		if (node is not JsonArray array || !TryGetCollectionItemType(propertyType, out var itemType))
			throw new InvalidOperationException($"Collection field [{field.FieldName}] in '{resourcePath}' must be an array.");

		var listType = typeof(List<>).MakeGenericType(itemType);
		var buffer = (IList)(Activator.CreateInstance(listType) ??
		                     throw new InvalidOperationException($"Failed to create collection buffer for '{propertyType.FullName}'."));

		foreach (var item in array)
		{
			if (item == null)
				continue;

			var itemValue = typeof(Def).IsAssignableFrom(itemType)
				? ReadReferenceOrObject(item, itemType, buffer, field, packageId, resourcePath, pendingReferences)
				: item is JsonObject obj && !IsScalarType(itemType)
					? ParseObject(itemType, obj, packageId, resourcePath, null, pendingReferences)
					: ReadScalar(GetScalar(item), itemType, field.FieldName, propertyType.Name, resourcePath);

			if (!ReferenceEquals(itemValue, PendingValue.Instance))
				buffer.Add(itemValue);
		}

		return MaterializeCollection(propertyType, itemType, buffer, resourcePath);
	}

	private Type ResolveObjectType(JsonObject obj, Type fallbackType, string resourcePath)
	{
		if (fallbackType is { IsAbstract: false, IsInterface: false })
			return fallbackType;

		var typeName = GetTypeName(obj) ??
		               throw new InvalidOperationException($"Polymorphic JSON object in '{resourcePath}' has no type marker.");

		return ResolveType(typeName, fallbackType, resourcePath);
	}

	private Type ResolveType(string name, Type expectedBaseType, string resourcePath)
	{
		if (!_namedTypes.TryGetValue(name, out var type) || !expectedBaseType.IsAssignableFrom(type))
		{
			throw new InvalidOperationException(
				$"Unknown definition type '{name}' assignable to '{expectedBaseType.FullName}' in '{resourcePath}'.");
		}

		return type;
	}

	private DefTypeMetadata GetTypeMetadata(Type type)
	{
		if (_metadataByType.TryGetValue(type, out var cached))
			return cached;

		var fields = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.Where(static property => property.CanWrite && property.GetIndexParameters().Length == 0)
			.Where(static property => property.Name is not nameof(Def.Id)
			                          && property.Name is not nameof(Def.PackageId)
			                          && property.Name is not nameof(Def.ResourcePath)
			                          && property.Name is not nameof(Def.ResourceName))
			.Select(static property =>
			{
				var attr = property.GetCustomAttribute<DefFieldAttribute>(inherit: true);
				return attr == null ? null : BuildFieldMetadata(property, attr);
			})
			.Where(static field => field != null)
			.Cast<FieldMetadata>()
			.ToArray();

		var metadata = new DefTypeMetadata(type, BuildFactory(type), fields);
		_metadataByType[type] = metadata;
		return metadata;
	}

	private static FieldMetadata BuildFieldMetadata(PropertyInfo property, DefFieldAttribute attr)
	{
		var setter = property.SetMethod ??
		             throw new InvalidOperationException($"Writable property '{property.Name}' has no set method.");

		return new FieldMetadata(
			property,
			property.PropertyType,
			attr,
			string.IsNullOrWhiteSpace(attr.Name) ? property.Name : attr.Name,
			(target, value) => setter.Invoke(target, [value]));
	}

	private static Func<object> BuildFactory(Type type)
	{
		return () => Activator.CreateInstance(type)
		             ?? throw new InvalidOperationException($"Failed to instantiate '{type.FullName}'.");
	}

	private static object ReadScalar(string? rawValue, Type targetType, string memberName, string ownerTypeName, string resourcePath)
	{
		if (string.IsNullOrWhiteSpace(rawValue))
			throw new InvalidOperationException($"Scalar field [{memberName}] on [{ownerTypeName}] in '{resourcePath}' is empty.");

		var target = Nullable.GetUnderlyingType(targetType) ?? targetType;
		var raw = rawValue.Trim();

		if (target == typeof(string))
			return raw;

		if (target == typeof(int) && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
			return intValue;

		if (target == typeof(uint) && uint.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uintValue))
			return uintValue;

		if (target == typeof(long) && long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
			return longValue;

		if (target == typeof(ulong) && ulong.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ulongValue))
			return ulongValue;

		if (target == typeof(short) && short.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var shortValue))
			return shortValue;

		if (target == typeof(byte) && byte.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var byteValue))
			return byteValue;

		if (target == typeof(float) && float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
			return floatValue;

		if (target == typeof(double) && double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
			return doubleValue;

		if (target == typeof(decimal) && decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var decimalValue))
			return decimalValue;

		if (target == typeof(bool) && bool.TryParse(raw, out var boolValue))
			return boolValue;

		if (target == typeof(DefId))
			return DefId.Parse(raw);

		if (target.IsEnum && Enum.TryParse(target, raw, ignoreCase: true, out var enumValue))
			return enumValue;

		throw new InvalidOperationException(
			$"Failed to parse value '{raw}' as '{targetType.Name}' for member '{memberName}' on type '{ownerTypeName}' in '{resourcePath}'.");
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
					return ex.Types.Where(static type => type != null).Cast<Type>();
				}
			})
			.Where(static type => type is { IsAbstract: false } && typeof(Def).IsAssignableFrom(type))
			.ToArray();

		var aliasGroups = allTypes
			.SelectMany(static type => GetTypeAliases(type).Select(alias => new { Alias = alias, Type = type }))
			.GroupBy(static x => x.Alias, StringComparer.OrdinalIgnoreCase)
			.ToArray();

		var conflicts = aliasGroups
			.Where(static group => group.Select(x => x.Type).Distinct().Count() > 1)
			.ToArray();

		if (conflicts.Length > 0)
		{
			var lines = conflicts.Select(static group =>
				$"Alias '{group.Key}' is declared by: {string.Join(", ", group.Select(x => x.Type.FullName).Distinct())}");

			throw new InvalidOperationException(
				"Duplicate json type aliases detected:" + Environment.NewLine +
				string.Join(Environment.NewLine, lines));
		}

		return aliasGroups.ToDictionary(static group => group.Key, static group => group.First().Type, StringComparer.OrdinalIgnoreCase);
	}

	private static IEnumerable<string> GetTypeAliases(Type type)
	{
		yield return type.Name;

		if (type.Name.EndsWith("Def", StringComparison.Ordinal) && type.Name.Length > "Def".Length)
			yield return type.Name[..^"Def".Length];

		var alias = type.GetCustomAttribute<DefTypeAttribute>(inherit: false)?.ElementName;
		if (!string.IsNullOrWhiteSpace(alias))
			yield return alias;
	}

	private static bool TryGetContainerArray(JsonObject obj, out JsonArray array)
	{
		foreach (var property in ContainerProperties)
		{
			if (GetProperty(obj, property) is JsonArray found)
			{
				array = found;
				return true;
			}
		}

		array = null!;
		return false;
	}

	private bool TryUnwrapTypedObject(JsonObject obj, out string typeName, out JsonObject wrapped)
	{
		if (obj.Count == 1)
		{
			var pair = obj.First();
			if (pair.Value is JsonObject value && _namedTypes.ContainsKey(pair.Key))
			{
				typeName = pair.Key;
				wrapped = value;
				return true;
			}
		}

		typeName = string.Empty;
		wrapped = null!;
		return false;
	}

	private static string? GetTypeName(JsonObject obj)
	{
		foreach (var property in TypeProperties)
		{
			var value = GetScalar(GetProperty(obj, property));
			if (!string.IsNullOrWhiteSpace(value))
				return value;
		}

		return null;
	}

	private static JsonNode? GetProperty(JsonObject obj, string name)
	{
		foreach (var (key, value) in obj)
		{
			if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
				return value;
		}

		return null;
	}

	private static string? GetScalar(JsonNode? node)
	{
		return node switch
		{
			null => null,
			JsonValue value when value.TryGetValue<string>(out var stringValue) => stringValue,
			JsonValue value => value.ToString(),
			_ => null
		};
	}

	private static bool TryGetCollectionItemType(Type propertyType, out Type itemType)
	{
		itemType = null!;

		if (propertyType == typeof(string))
			return false;

		if (propertyType.IsArray)
		{
			itemType = propertyType.GetElementType()!;
			return itemType != null;
		}

		var enumerable = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
			? propertyType
			: propertyType.GetInterfaces()
				.FirstOrDefault(static type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>));

		if (enumerable == null)
			return false;

		itemType = enumerable.GetGenericArguments()[0];
		return true;
	}

	private static object MaterializeCollection(Type propertyType, Type itemType, IList buffer, string resourcePath)
	{
		if (propertyType.IsArray)
		{
			var array = Array.CreateInstance(itemType, buffer.Count);
			buffer.CopyTo(array, 0);
			return array;
		}

		if (propertyType.IsInstanceOfType(buffer))
			return buffer;

		if (propertyType is { IsInterface: true, IsGenericType: true })
		{
			var genericDef = propertyType.GetGenericTypeDefinition();
			if (genericDef == typeof(IEnumerable<>) ||
			    genericDef == typeof(IReadOnlyCollection<>) ||
			    genericDef == typeof(IReadOnlyList<>) ||
			    genericDef == typeof(ICollection<>) ||
			    genericDef == typeof(IList<>))
			{
				return buffer;
			}
		}

		var collection = Activator.CreateInstance(propertyType)
		                 ?? throw new InvalidOperationException(
			                 $"Failed to instantiate collection type '{propertyType.FullName}' in '{resourcePath}'.");

		var addMethod = collection.GetType().GetMethod("Add", [itemType]);
		if (addMethod == null)
		{
			throw new InvalidOperationException(
				$"Collection type '{propertyType.FullName}' does not expose Add({itemType.Name}) in '{resourcePath}'.");
		}

		foreach (var item in buffer)
			addMethod.Invoke(collection, [item]);

		return collection;
	}

	private static bool IsScalarType(Type type)
	{
		var target = Nullable.GetUnderlyingType(type) ?? type;
		return target.IsPrimitive ||
		       target.IsEnum ||
		       target == typeof(string) ||
		       target == typeof(decimal) ||
		       target == typeof(DefId);
	}

	private sealed record DefTypeMetadata(Type Type, Func<object> Factory, IReadOnlyList<FieldMetadata> Fields);

	private sealed record FieldMetadata(
		PropertyInfo Property,
		Type PropertyType,
		DefFieldAttribute Attribute,
		string FieldName,
		Action<object, object?> Setter);

	private sealed class PendingValue
	{
		public static readonly PendingValue Instance = new();

		private PendingValue()
		{
		}
	}
}
