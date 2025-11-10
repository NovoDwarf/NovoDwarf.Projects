using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Utilities.Services.Files;

public partial class FileService
{
	private readonly ILogger<FileService> _logger;

	public FileService(ILogger<FileService> logger)
	{
		_logger = logger;
	}
	
	public string Read(string path)
	{
		if (!File.Exists(path))
			throw new FileNotFoundException($"File not found at [{path}]");
		
		var file = File.ReadAllText(path);

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
		
		File.WriteAllText(path, content);
	}
}