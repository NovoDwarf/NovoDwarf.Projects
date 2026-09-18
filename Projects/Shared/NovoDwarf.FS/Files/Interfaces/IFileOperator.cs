namespace NovoDwarf.FS.Files.Interfaces;

public interface IFileOperator
{
	public void Copy(string sourceFileName, string destFileName);

	public void Copy(string sourceFileName, string destFileName, bool overwrite);

	public void Move(string sourceFileName, string destFileName);

	public void Move(string sourceFileName, string destFileName, bool overwrite);

	public void Delete(string path);

	public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName);

	public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors);

	public FileStream Create(string path);

	public FileStream Create(string path, int bufferSize);

	public FileStream Create(string path, int bufferSize, FileOptions options);
}