namespace NovoDwarf.FS.Directories.Interfaces;

public interface IDirectoryWriter
{
	/// <inheritdoc cref="Directory.CreateDirectory(string)"/>
	public DirectoryInfo CreateDirectory(string path);

	public DirectoryInfo CreateSubdirectory(string path, string subdirectoryName);

	/// <inheritdoc cref="Directory.Delete(string, bool)"/>
	public void Delete(string path, bool recursive = false);

	/// <inheritdoc cref="Directory.Move"/>
	public void Move(string sourceDirName, string destDirName);

	/// <inheritdoc cref="Directory.SetCurrentDirectory"/>
	public void SetCurrentDirectory(string path);

	/// <inheritdoc cref="Directory.CreateTempSubdirectory"/>
	public DirectoryInfo CreateTempSubdirectory(string? prefix = null);
}