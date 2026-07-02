using System.Xml.Linq;

namespace Aegis.Packaging.Definitions.Reader.Parsers;

internal static class DefinitionXmlValueParsers
{
	public static string? GetAttribute(XElement element, string name)
	{
		return element.Attributes()
			.FirstOrDefault(attribute => string.Equals(attribute.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
			?.Value;
	}

	public static XElement? GetChild(XElement element, string name)
	{
		return element.Elements()
			.FirstOrDefault(child => string.Equals(child.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));
	}

	public static string GetValue(XElement element)
	{
		if (element.HasElements)
			throw new InvalidOperationException($"Element '{element.Name.LocalName}' contains nested elements and cannot be parsed as a Point value.");

		return element.Attribute("value")?.Value?.Trim() ?? element.Value.Trim();
	}
}
