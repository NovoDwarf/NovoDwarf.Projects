using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NovoDwarf.FS.Interfaces;

namespace NovoDwarf.FS;

public sealed class DefaultFileService : IFileService
{

	#region Base Methods (Sync)

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ReadAllText(string path) => File.ReadAllText(path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WriteAllText(string path, string content) => File.WriteAllText(path, content);
	
	#endregion
	
	#region Base Methods (Async)

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public async Task<string> ReadAllTextAsync(string path) => await File.ReadAllTextAsync(path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public async Task WriteAllTextAsync(string path, string content) => await File.WriteAllTextAsync(path, content);

	#endregion

	#region Search Methods

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool FileExists(string path) => File.Exists(path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool DirectoryExists(string path) => Directory.Exists(path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories) 
		=> Directory.EnumerateFiles(path, searchPattern, searchOption);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories) 
		=> Directory.EnumerateDirectories(path, searchPattern, searchOption);

	#endregion

	#region Application Methods (Sync)

	public string Read(string path)
	{
		if (!FileExists(path))
			throw new FileNotFoundException($"File not found at [{path}]");

		var file = ReadAllText(path);

		if (string.IsNullOrWhiteSpace(file))
			throw new NullReferenceException($"File is null or whitespace at [{path}]");

		return file;
	}

	public void Write(string path, string content)
	{
		if (string.IsNullOrWhiteSpace(path))
			throw new NullReferenceException($"Path is null or whitespace: [{path}]");

		if (string.IsNullOrWhiteSpace(content))
			throw new NullReferenceException($"Content is null or whitespace: [{path}]");

		WriteAllText(path, content);
	}

	#endregion

	#region Application Methods (Async)

	public async Task<string> ReadAsync(string path)
	{
		if (!FileExists(path))
			throw new FileNotFoundException($"File not found at {path}");

		var file = await ReadAllTextAsync(path);

		if (string.IsNullOrWhiteSpace(file))
			throw new NullReferenceException($"JSON is invalid at {path}");

		return file;
	}

	public async Task WriteAsync(string path, string content)
	{
		if (string.IsNullOrWhiteSpace(path))
			throw new NullReferenceException($"Path is null or whitespace: {path}");

		if (string.IsNullOrWhiteSpace(content))
			throw new NullReferenceException($"Content is null or whitespace: {path}");

		await WriteAllTextAsync(path, content);
	}

	#endregion
}