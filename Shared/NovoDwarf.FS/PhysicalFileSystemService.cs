using NovoDwarf.FS.Interfaces;

namespace NovoDwarf.FS;

public sealed class PhysicalFileSystemService : IFileSystemService
{
	public string ReadAllText(string path)
	{
		return File.ReadAllText(path);
	}

	public bool FileExists(string path)
	{
		return File.Exists(path);
	}

	public bool DirectoryExists(string path)
	{
		return Directory.Exists(path);
	}

	public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
	{
		return Directory.EnumerateFiles(path, searchPattern, searchOption);
	}
}