using System.Text;
using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc/>
public sealed class DefaultFileReader : IFileReader
{
	#region Read Methods

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ReadAllText(string path) => File.ReadAllText(path);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ReadAllText(string path, Encoding encoding) => File.ReadAllText(path, encoding);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string[] ReadAllLines(string path) => File.ReadAllLines(path);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string[] ReadAllLines(string path, Encoding encoding) => File.ReadAllLines(path, encoding);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<string> ReadLines(string path) => File.ReadLines(path);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<string> ReadLines(string path, Encoding encoding) => File.ReadLines(path, encoding);

	#endregion
	
	#region Async Read Methods

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
		=> File.ReadAllTextAsync(path, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
		=> File.ReadAllTextAsync(path, encoding, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default)
		=> File.ReadAllLinesAsync(path, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
		=> File.ReadAllLinesAsync(path, encoding, cancellationToken);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
		=> File.ReadAllBytesAsync(path, cancellationToken);

	#endregion

}