using System.Reflection;

namespace Aegis.Packaging.Assemblies;

internal static class GodotScriptLookup
{
	private static readonly object Gate = new();
	private static MethodInfo? _lookupScriptsInAssembly;
	private static MethodInfo? _lookupScriptsInReferencedAssemblies;

	public static void LookupScripts(IReadOnlyList<Assembly> assemblies, string? primaryAssemblyPath)
	{
		if (assemblies.Count == 0)
			return;

		EnsureInitialized();

		// If GodotSharp isn't available (e.g. headless tools), no-op.
		if (_lookupScriptsInAssembly == null)
			return;

		foreach (var assembly in assemblies)
		{
			try
			{
				_lookupScriptsInAssembly.Invoke(null, [assembly]);
			}
			catch
			{
				// Best-effort: failing to register one assembly shouldn't prevent package loading.
			}
		}

		if (_lookupScriptsInReferencedAssemblies != null &&
		    !string.IsNullOrWhiteSpace(primaryAssemblyPath))
		{
			try
			{
				// Helps multi-assembly packages (ProjectReference) if they contain scripts.
				_lookupScriptsInReferencedAssemblies.Invoke(null, [assemblies[0], primaryAssemblyPath]);
			}
			catch
			{
				// Best-effort.
			}
		}
	}

	private static void EnsureInitialized()
	{
		if (_lookupScriptsInAssembly != null)
			return;

		lock (Gate)
		{
			if (_lookupScriptsInAssembly != null)
				return;

			var bridgeType = Type.GetType("Godot.Bridge.ScriptManagerBridge, GodotSharp");
			if (bridgeType == null)
				return;

			_lookupScriptsInAssembly = bridgeType.GetMethod(
				"LookupScriptsInAssembly",
				BindingFlags.Public | BindingFlags.Static,
				binder: null,
				types: [typeof(Assembly)],
				modifiers: null);

			_lookupScriptsInReferencedAssemblies = bridgeType.GetMethod(
				"LookupScriptsInReferencedAssemblies",
				BindingFlags.Public | BindingFlags.Static,
				binder: null,
				types: [typeof(Assembly), typeof(string)],
				modifiers: null);
		}
	}
}

