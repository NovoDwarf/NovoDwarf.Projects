namespace NovoDwarf.FS.Files.Interfaces;

public interface IFileSearcher
{
	public string[] GetFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	public IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	public bool Exists(string path);
}