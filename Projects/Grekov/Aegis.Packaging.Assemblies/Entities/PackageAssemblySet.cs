namespace Aegis.Packaging.Assemblies.Entities;

public sealed record PackageAssemblySet(string AssembliesRoot, string PrimaryAssemblyPath, IReadOnlyList<string> AssemblyPaths);