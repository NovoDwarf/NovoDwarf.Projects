namespace NovoDwarf.FS.Directories.Interfaces;

public interface IDirectoryMetadata
{
	/// <inheritdoc cref="Directory.CreateSymbolicLink"/>
	public FileSystemInfo CreateSymbolicLink(string path, string pathToTarget);
	
	/// <inheritdoc cref="Directory.ResolveLinkTarget"/>
	public FileSystemInfo? ResolveLinkTarget(string path, bool returnFinalTarget);
	
	/// <inheritdoc cref="Directory.GetCreationTime(string)"/>
	public DateTime GetCreationTime(string path);
	
	/// <inheritdoc cref="Directory.GetCreationTimeUtc(string)"/>
	public DateTime GetCreationTimeUtc(string path);

	/// <inheritdoc cref="Directory.SetCreationTime(string, DateTime)"/>
	public void SetCreationTime(string path, DateTime creationTime);
	
	/// <inheritdoc cref="Directory.SetCreationTimeUtc(string, DateTime)"/>
	public void SetCreationTimeUtc(string path, DateTime creationTimeUtc);

	/// <inheritdoc cref="Directory.GetLastAccessTime(string)"/>
	public DateTime GetLastAccessTime(string path);
	
	/// <inheritdoc cref="Directory.GetLastAccessTimeUtc(string)"/>
	public DateTime GetLastAccessTimeUtc(string path);

	/// <inheritdoc cref="Directory.SetLastAccessTime(string, DateTime)"/>
	public void SetLastAccessTime(string path, DateTime lastAccessTime);

	/// <inheritdoc cref="Directory.SetLastAccessTimeUtc(string, DateTime)"/>
	public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

	/// <inheritdoc cref="Directory.GetLastWriteTime(string)"/>
	public DateTime GetLastWriteTime(string path);

	/// <inheritdoc cref="Directory.GetLastWriteTimeUtc(string)"/>
	public DateTime GetLastWriteTimeUtc(string path);

	/// <inheritdoc cref="Directory.SetLastWriteTime(string, DateTime)"/>
	public void SetLastWriteTime(string path, DateTime lastWriteTime);

	/// <inheritdoc cref="Directory.SetLastWriteTimeUtc(string, DateTime)"/>
	public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
}