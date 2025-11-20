using Microsoft.CodeAnalysis;

namespace Mathematics.SourceGenerators.Utilities;

public static class DiagnosticUtils
{
	internal static DiagnosticDescriptor ErrorGenerating()
	{
		return new DiagnosticDescriptor(
			id: "MSG001",
			title: "Error generating properties",
			messageFormat: "Error generating properties for {0}: {1}",
			category: "Generation",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Error generating properties for {0}: {1}");
	}
	
	internal static DiagnosticDescriptor ErrorGeneratingDistributionConstructor()
	{
		return new DiagnosticDescriptor(
			id: "MSG002",
			title: "Error generating constructor",
			messageFormat: "Error generating constructor for {0}: {1}",
			category: "Generation",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Error generating constructor for {0}: {1}");
	}
	
	internal static DiagnosticDescriptor ErrorGeneratingDistributionController()
	{
		return new DiagnosticDescriptor(
			id: "MSG003",
			title: "Error generating controller",
			messageFormat: "Error generating controller for {0}: {1}",
			category: "Generation",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Error generating controller for {0}: {1}");
	}
}