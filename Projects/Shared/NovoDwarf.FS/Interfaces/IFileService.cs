using System.Collections.Generic;
using System.IO;

namespace NovoDwarf.FS.Interfaces;

public interface IFileService
{
	public string ReadAllText(string path);
	public void WriteAllText(string path, string content);
	
	public bool FileExists(string path);
	public bool DirectoryExists(string path);
	
	public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
	public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories);

}
