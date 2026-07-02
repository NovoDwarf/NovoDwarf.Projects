using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Aegis.Packaging.Assemblies.Extensions;
using Aegis.Packaging.Assemblies.Utilities;
using Aegis.Packaging.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Assemblies;

public sealed class EntrypointResolver
{
	private readonly ILogger<AssemblyService> _logger;

	public EntrypointResolver(ILogger<AssemblyService> logger)
	{
		_logger = logger;
	}

	public Type[] ResolveEntrypointTypes(
		PackageInstance package,
		AssemblyLoadContext loadContext,
		IReadOnlyList<string> assemblyPaths,
		string primaryAssemblyPath)
	{
		var assemblies = new List<Assembly>(assemblyPaths.Count);

		assemblies.AddRange(PackageAssemblyLocator
			.OrderAssemblyPaths(package, assemblyPaths)
			.Select(assemblyPath => LoadAssembly(loadContext, assemblyPath)));

		// Ensure Godot can resolve C# script classes by res:// path for dynamically loaded assemblies.
		// Without this, PackedScene instantiation fails with "associated class could not be found".
		GodotScriptLookup.LookupScripts(assemblies, primaryAssemblyPath);

		var allTypes = assemblies
			.SelectMany(GetLoadableTypes)
			.ToArray();

		var discoveredEntrypoints = allTypes
			.Where(IsEntrypointType)
			.ToArray();

		if (discoveredEntrypoints.Length == 0)
		{
			var details = BuildEntrypointDiscoveryDiagnostic(package, assemblies, allTypes);
			_logger.FailedEntrypointDiscovery(package.Id, primaryAssemblyPath, details);

			ExceptionUtils.ThrowIfNoEntrypoints(package.Id, details);
		}

		if (discoveredEntrypoints.Length > 1)
		{
			var names = string.Join(", ", discoveredEntrypoints.Select(static t => t.FullName));
			ExceptionUtils.ThrowIfManyEntrypoints(package.Id, 1, names);
		}

		return discoveredEntrypoints;
	}

	private static bool IsEntrypointType(Type type)
	{
		return typeof(IEntrypoint).IsAssignableFrom(type) && type is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false };
	}

	private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			return ex.Types.Where(static t => t != null)!;
		}
	}

	private static Assembly LoadAssembly(AssemblyLoadContext loadContext, string assemblyPath)
	{
		var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
		var shared = AssemblyLoadPolicy.FindLoadedAssembly(assemblyName.Name);

		return shared ?? loadContext.LoadFromAssemblyPath(assemblyPath);
	}

	private static string BuildEntrypointDiscoveryDiagnostic(
		PackageInstance package,
		IReadOnlyList<Assembly> assemblies,
		IReadOnlyList<Type> allTypes)
	{
		var hostInterface = typeof(IEntrypoint);
		var hostInterfaceAssembly = hostInterface.Assembly.GetName().Name
		                            ?? hostInterface.Assembly.FullName
		                            ?? "<unknown>";

		var suspectTypes = allTypes
			.Where(static type => type is { IsAbstract: false, IsInterface: false })
			.Where(type =>
				string.Equals(type.Name, "Entrypoint", StringComparison.OrdinalIgnoreCase) ||
				string.Equals(type.FullName, $"{package.Id}.Entrypoint", StringComparison.OrdinalIgnoreCase) ||
				type.GetInterfaces().Any(i =>
					string.Equals(i.FullName, hostInterface.FullName, StringComparison.Ordinal)))
			.OrderBy(static type => type.FullName, StringComparer.Ordinal)
			.Take(8)
			.ToArray();

		var sb = new StringBuilder();
		sb.Append("Host interface assembly=");
		sb.Append(hostInterfaceAssembly);
		sb.Append(". Loaded assemblies=");
		sb.Append(string.Join(", ", assemblies.Select(static assembly => assembly.GetName().Name)));

		if (suspectTypes.Length == 0)
		{
			sb.Append(". No candidate entrypoint-like types were found.");
			return sb.ToString();
		}

		sb.Append(". Candidates: ");
		sb.Append(string.Join("; ", suspectTypes.Select(DescribeType)));
		return sb.ToString();
	}

	private static string DescribeType(Type type)
	{
		var typeAssembly = type.Assembly.GetName().Name ?? type.Assembly.FullName ?? "<unknown>";
		var interfaces = type.GetInterfaces()
			.Select(static iface => $"{iface.FullName} [{iface.Assembly.GetName().Name}]")
			.OrderBy(static value => value, StringComparer.Ordinal)
			.ToArray();

		return $"{type.FullName} [{typeAssembly}] -> {string.Join(", ", interfaces)}";
	}
}
