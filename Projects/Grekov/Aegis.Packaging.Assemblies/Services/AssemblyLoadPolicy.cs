using System.Reflection;

namespace Aegis.Packaging.Assemblies;

public static class AssemblyLoadPolicy
{
	private static readonly HashSet<string> ReservedHostAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Aegis.Api",
		"Aegis.Api.Godot",
		"Aegis.Api.Internal",
		"Aegis.Core",
		"Aegis.Game.Contracts",
		"Aegis.Networking",
		"Aegis.Definitions.Abstractions",
		"Aegis.Definitions.Core",
		"Aegis.Numerics"
	};

	public static bool IsReservedHostAssemblyName(string? assemblyName, string packageAssemblyName)
	{
		return !string.IsNullOrWhiteSpace(assemblyName) && ReservedHostAssemblyNames.Contains(assemblyName) &&
		       !string.Equals(assemblyName, packageAssemblyName, StringComparison.OrdinalIgnoreCase);
	}

	public static Assembly? FindLoadedAssembly(string? assemblyName)
	{
		if (string.IsNullOrWhiteSpace(assemblyName))
			return null;

		return AppDomain.CurrentDomain.GetAssemblies()
			.FirstOrDefault(assembly => string.Equals(assembly.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
	}
}
