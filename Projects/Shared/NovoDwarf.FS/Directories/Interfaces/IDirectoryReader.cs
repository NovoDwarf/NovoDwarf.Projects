namespace NovoDwarf.FS.Directories.Interfaces;

public interface IDirectoryReader
{
	/// <inheritdoc cref="Directory.Exists"/>
	public bool Exists(string? path);

	public DirectoryInfo? GetInfo(string path);

	/// <inheritdoc cref="Directory.GetDirectories(string, string, SearchOption)"/>
	public string[] GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	/// <inheritdoc cref="Directory.EnumerateDirectories(string, string, SearchOption)"/>
	public IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	/// <inheritdoc cref="Directory.GetFiles(string, string, SearchOption)"/>
	public string[] GetFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	/// <inheritdoc cref="Directory.EnumerateFiles(string, string, SearchOption)"/>
	public IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	/// <inheritdoc cref="Directory.GetFileSystemEntries(string, string, SearchOption)"/>
	public string[] GetFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	/// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string, string, SearchOption)"/>
	public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

	/// <inheritdoc cref="Directory.GetLogicalDrives"/>
	public string[] GetLogicalDrives();

	/// <inheritdoc cref="Directory.GetParent"/>
	public DirectoryInfo? GetParent(string path);

	/// <inheritdoc cref="Directory.GetCurrentDirectory"/>
	public string GetCurrentDirectory();
}