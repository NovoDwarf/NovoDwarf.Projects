namespace NovoDwarf.FS.Files.Interfaces;

public interface IFileMetadata
{
	public void Encrypt(string path);

	public void Decrypt(string path);

	public void CreateSymbolicLink(string path, string pathToTarget);

	public FileSystemInfo? ResolveLinkTarget(string linkPath, bool returnFinalTarget);
	
	public bool Exists(string path);

	public FileAttributes GetAttributes(string path);

	public void SetAttributes(string path, FileAttributes fileAttributes);

	public DateTime GetCreationTime(string path);

	public DateTime GetCreationTimeUtc(string path);

	public void SetCreationTime(string path, DateTime creationTime);

	public void SetCreationTimeUtc(string path, DateTime creationTimeUtc);

	public DateTime GetLastAccessTime(string path);

	public DateTime GetLastAccessTimeUtc(string path);

	public void SetLastAccessTime(string path, DateTime lastAccessTime);

	public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

	public DateTime GetLastWriteTime(string path);

	public DateTime GetLastWriteTimeUtc(string path);

	public void SetLastWriteTime(string path, DateTime lastWriteTime);

	public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
}