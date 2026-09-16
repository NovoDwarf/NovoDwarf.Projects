using System.IO;
using NovoDwarf.FS.Interfaces;

namespace NovoDwarf.FS;

public sealed class DefaultPathService : IPathService
{
	public string GetExtension(string path)
	{
		return Path.GetExtension(path);
	}

	public string GetDirectoryName(string path)
	{
		return Path.GetFileName(Path.TrimEndingDirectorySeparator(path));
	}
	
	public string Combine(params string[] paths)
	{
		return Path.Combine(paths);
	}
}
