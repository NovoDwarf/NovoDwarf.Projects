namespace NovoDwarf.FS.Files.Interfaces;

public interface IFileStreamer
{
	public StreamWriter AppendText(string path);

	public FileStream Open(string path, FileMode mode);

	public FileStream Open(string path, FileMode mode, FileAccess access);

	public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share);

	public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize);

	public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options);

	public FileStream OpenRead(string path);

	public FileStream OpenWrite(string path);
	
	public StreamReader OpenText(string path);
}