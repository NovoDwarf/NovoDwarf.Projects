using System.Collections;
using System.Xml.Linq;

namespace Aegis.Packaging.Definitions.Reader.Parsers;

internal static class DefinitionXmlCollectionParser
{
	public static Type? GetCollectionItemType(Type propertyType)
	{
		if (propertyType == typeof(string))
			return null;

		if (propertyType.IsArray)
			return propertyType.GetElementType();

		if (propertyType.IsInterface && propertyType.IsGenericType)
		{
			var genericDef = propertyType.GetGenericTypeDefinition();
			if (genericDef == typeof(IEnumerable<>) ||
			    genericDef == typeof(IReadOnlyCollection<>) ||
			    genericDef == typeof(IReadOnlyList<>) ||
			    genericDef == typeof(ICollection<>) ||
			    genericDef == typeof(IList<>))
			{
				return propertyType.GetGenericArguments()[0];
			}
		}

		var enumerableInterface = propertyType.IsGenericType &&
		                          propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
			? propertyType
			: propertyType
			.GetInterfaces()
			.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

		return enumerableInterface?.GetGenericArguments()[0];
	}

	public static IEnumerable<XElement> EnumerateCollectionItems(XElement container, DefFieldAttribute field)
	{
		return string.IsNullOrWhiteSpace(field.ItemName)
			? container.Elements()
			: container.Elements().Where(element =>
				string.Equals(element.Name.LocalName, field.ItemName, StringComparison.OrdinalIgnoreCase));
	}

	public static object MaterializeCollection(Type propertyType, Type itemType, IList buffer, string resourcePath)
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
}
