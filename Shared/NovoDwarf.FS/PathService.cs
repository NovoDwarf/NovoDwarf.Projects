using NovoDwarf.FS.Interfaces;

namespace NovoDwarf.FS;

public sealed class PathService : IPathService
{
	public string GetExtension(string path)
	{
		return Path.GetExtension(path);
	}

	public string Combine(params string[] paths)
	{
		return Path.Combine(paths);
	}
}
