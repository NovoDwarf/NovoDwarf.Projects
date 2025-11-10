using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Utilities.Services.Files;

public partial class FileService
{
	public async Task<IReadOnlyCollection<string>> AllAsync(string root, string searchPattern, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(root))
			throw new NullReferenceException($"Root is null or whitespace: {root}");
		
		if (!Directory.Exists(root))
			throw new DirectoryNotFoundException($"Directory not found at {root}");
		
		var result = new ConcurrentBag<string>();
		
		await TraverseAsync(root, searchPattern, result, cancellationToken);
		
		return result;
	}
	
	private async Task TraverseAsync(string path, string searchPattern, ConcurrentBag<string> result, CancellationToken cancellationToken)
	{
		try
		{
			foreach (var file in Directory.EnumerateFiles(path, searchPattern))
			{
				cancellationToken.ThrowIfCancellationRequested();
				result.Add(file);
			}
			
			await Parallel.ForEachAsync(Directory.EnumerateDirectories(path), cancellationToken, async (dir, ct) =>
			{
				await TraverseAsync(dir, searchPattern, result, ct);
			});
		}
		catch (UnauthorizedAccessException ex)
		{
			_logger.LogWarning(ex, "Access denied: [{Path}]", path);
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error traversing at [{Path}]", path);
		}
	}
}