using System.Text;
using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc/>
public class DefaultFileWriter : IFileWriter
{
	#region Write Methods

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteAllText(string path, string? contents) => File.WriteAllText(path, contents);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteAllText(string path, string? contents, Encoding encoding) => File.WriteAllText(path, contents, encoding);
	
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteAllBytes(string path, byte[] bytes) => File.WriteAllBytes(path, bytes);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteAllBytes(string path, ReadOnlySpan<byte> bytes) => File.WriteAllBytes(path, bytes);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteAllLines(string path, IEnumerable<string> contents) => File.WriteAllLines(path, contents);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding) => File.WriteAllLines(path, contents, encoding);

	#endregion
	
	#region Async Write Methods

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default)
		=> File.WriteAllTextAsync(path, contents, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task WriteAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default)
		=> File.WriteAllTextAsync(path, contents, encoding, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
		=> File.WriteAllBytesAsync(path, bytes, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task WriteAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
		=> File.WriteAllBytesAsync(path, bytes, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default)
		=> File.WriteAllLinesAsync(path, contents, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default)
		=> File.WriteAllLinesAsync(path, contents, encoding, cancellationToken);

	#endregion
}