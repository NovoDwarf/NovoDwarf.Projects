using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Utilities.Services.Files;

public partial class FileService
{
	public string First(string root, string searchPattern)
	{
		if (string.IsNullOrWhiteSpace(root))
			throw new NullReferenceException($"Root is null or whitespace: {root}");
		
		if (!Directory.Exists(root))
			throw new DirectoryNotFoundException($"Directory not found at {root}");

		return Directory.EnumerateFiles(root, searchPattern).First();
	}
	
	public List<string> All(string root, string searchPattern)
	{
		if (string.IsNullOrWhiteSpace(root))
			throw new NullReferenceException($"Root is null or whitespace: {root}");
		
		if (!Directory.Exists(root))
			throw new DirectoryNotFoundException($"Directory not found at {root}");
		
		var result = new ConcurrentBag<string>();
		
		Traverse(root, searchPattern, result);
		 
		return result.ToList();
	}
	
	private void Traverse(string path, string searchPattern, ConcurrentBag<string> result)
	{
		try
		{
			foreach (var file in Directory.EnumerateFiles(path, searchPattern))
			{
				result.Add(file);
			}
			
			foreach (var dir in Directory.EnumerateDirectories(path))
			{
				Traverse(dir, searchPattern, result);
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			_logger.LogWarning(ex, "Нет доступа к {Path}", path);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при обходе {Path}", path);
		}
	}
}