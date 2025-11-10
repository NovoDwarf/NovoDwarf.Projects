using System;
using System.IO;
using System.Threading.Tasks;

namespace Utilities.Services.Files;

public partial class FileService
{
	public async Task<string> ReadAsync(string path)
	{
		if (!File.Exists(path))
			throw new FileNotFoundException($"File not found at {path}");
		
		var file = await File.ReadAllTextAsync(path);

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
		
		await File.WriteAllTextAsync(path, content);
	}
}