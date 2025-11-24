using Microsoft.CodeAnalysis;

namespace Mathematics.SourceGenerators.Utilities;

public static class ParseUtils
{
	public static string GenerateDouble(string paramName) 
		=> GenerateAssignment(paramName, "double", "double.TryParse({0}Str, out var {0}Value) ? {0}Value : default");

	public static string GenerateDoubleArray(string paramName) 
		=> GenerateAssignment(paramName, "double[]", "{0}Str.Split(',').Select(s => double.Parse(s.Trim())).ToArray()");

	public static string GenerateNullableDoubleArray(string paramName) 
		=> GenerateAssignment(paramName, "double[]?", "{0}Str?.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => double.Parse(s.Trim())).ToArray()");
    
	public static string GenerateInt(string paramName)
		=> GenerateAssignment(paramName, "int", "int.TryParse({0}Str, out var {0}Value) ? {0}Value : default");

	public static string GenerateGeneric(string paramName, string paramType) 
		=> GenerateAssignment(paramName, paramType, "({1})Convert.ChangeType({0}Str, typeof({1}))");
    
	private static string GenerateAssignment(string paramName, string paramType, string parseExpression)
	{
		var finalParseExpression = string.Format(parseExpression, paramName, paramType);
        
		return $$"""
		             if (request.Params.TryGetValue("{{paramName}}", out var {{paramName}}Str))
		             {
		                 {{paramName}} = {{finalParseExpression}};
		                 hasParams = true;
		             }
		         """;
	}
}