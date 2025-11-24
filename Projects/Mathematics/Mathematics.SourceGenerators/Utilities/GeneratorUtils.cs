using Microsoft.CodeAnalysis;

namespace Mathematics.SourceGenerators.Utilities;

public class GeneratorUtils
{
	private static readonly List<string> ClassSuffixes =
	[
		"Distribution", "Function"
	];
	
	internal static string GetCleanName(string name)
	{
		return ClassSuffixes
			.Aggregate(name, (current, suffix) => current.Replace(suffix, string.Empty))
			.ToLowerInvariant();
	}
	
	internal static bool HasValidSuffix(string className)
	{
		return ClassSuffixes.Any(suffix =>
			className.EndsWith(suffix) && className.Length > suffix.Length);
	}
	
	internal static bool IsDistributionSubclass(INamedTypeSymbol? classSymbol) => IsSubclass(classSymbol, "Distribution");
	
	internal static bool IsEntitySubclass(INamedTypeSymbol? classSymbol) => IsSubclass(classSymbol, "Entity");

	internal static bool IsSubclass(INamedTypeSymbol? classSymbol, string className)
	{
		if (classSymbol == null) 
			return false;
		
		var baseType = classSymbol.BaseType;
		
		while (baseType != null)
		{
			if (string.Equals(baseType.Name, className, StringComparison.OrdinalIgnoreCase))
				return true;

			baseType = baseType.BaseType;
		}

		return false;
	}
}