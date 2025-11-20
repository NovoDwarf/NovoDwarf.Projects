using Microsoft.CodeAnalysis;

namespace Mathematics.SourceGenerators.Utilities;

public static class ParseUtils
{
	public static string Parse(string valueAccess, string typeName, ITypeSymbol? parameterType = null)
	{
		return typeName switch
		{
			"string" => valueAccess,
			"int" or "System.Int32" => $"int.Parse({valueAccess})",
			"long" or "System.Int64" => $"long.Parse({valueAccess})",
			"double" or "System.Double" => $"double.Parse({valueAccess}, System.Globalization.CultureInfo.InvariantCulture)",
			"float" or "System.Single" => $"float.Parse({valueAccess}, System.Globalization.CultureInfo.InvariantCulture)",
			"bool" or "System.Boolean" => $"bool.Parse({valueAccess})",
			"decimal" or "System.Decimal" => $"decimal.Parse({valueAccess}, System.Globalization.CultureInfo.InvariantCulture)",
			"DateTime" or "System.DateTime" => $"System.DateTime.Parse({valueAccess}, System.Globalization.CultureInfo.InvariantCulture)",
			"Guid" or "System.Guid" => $"System.Guid.Parse({valueAccess})",
			"double[]" => $"System.Array.ConvertAll({valueAccess}.Split(','), double.Parse)",
			_ when parameterType?.TypeKind == TypeKind.Enum => 
				$"({typeName})System.Enum.Parse(typeof({typeName}), {valueAccess})",
			_ => $"({typeName}){valueAccess}"
		};
	}
}