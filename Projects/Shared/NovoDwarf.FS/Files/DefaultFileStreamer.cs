using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc/>
public class DefaultFileStreamer : IFileStreamer
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StreamWriter AppendText(string path) => File.AppendText(path);
	
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Open(string path, FileMode mode)
		=> File.Open(path, mode);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Open(string path, FileMode mode, FileAccess access)
		=> File.Open(path, mode, access);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
		=> File.Open(path, mode, access, share);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
		=> new(path, mode, access, share, bufferSize);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
		=> new(path, mode, access, share, bufferSize, options);
	
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream OpenRead(string path) => File.OpenRead(path);
	
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream OpenWrite(string path) => File.OpenWrite(path);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StreamReader OpenText(string path) => File.OpenText(path);
}