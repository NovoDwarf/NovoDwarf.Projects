namespace NovoDwarf.FS.Interfaces;

public interface IFileSystemService
{
	string ReadAllText(string path);
	bool FileExists(string path);
	bool DirectoryExists(string path);
	IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
}
