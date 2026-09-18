using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc/>
public sealed class DefaultFileOperator : IFileOperator 
{
	#region CRUD

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Copy(string sourceFileName, string destFileName) => File.Copy(sourceFileName, destFileName);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Copy(string sourceFileName, string destFileName, bool overwrite) => File.Copy(sourceFileName, destFileName, overwrite);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Move(string sourceFileName, string destFileName) => File.Move(sourceFileName, destFileName);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Move(string sourceFileName, string destFileName, bool overwrite) => File.Move(sourceFileName, destFileName, overwrite);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Delete(string path) => File.Delete(path);
	
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName)
		=> File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors)
		=> File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);

	#endregion
	
	#region File Creation

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Create(string path) => File.Create(path);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Create(string path, int bufferSize) => File.Create(path, bufferSize);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileStream Create(string path, int bufferSize, FileOptions options) => File.Create(path, bufferSize, options);

	#endregion
}