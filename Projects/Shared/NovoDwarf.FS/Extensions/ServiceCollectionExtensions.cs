using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NovoDwarf.FS.Directories;
using NovoDwarf.FS.Directories.Interfaces;
using NovoDwarf.FS.Files;
using NovoDwarf.FS.Files.Interfaces;
using NovoDwarf.FS.Paths;
using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddFileSystemServices()
		{
			services.AddDirectoryServices();
			services.AddFileServices();
			services.AddPathServices();
		
			return services;
		}

		public IServiceCollection AddDirectoryServices()
		{
			services.TryAddSingleton<IDirectoryMetadata, DefaultDirectoryMetadata>();
			services.TryAddSingleton<IDirectoryReader, DefaultDirectoryReader>();
			services.TryAddSingleton<IDirectoryWriter, DefaultDirectoryWriter>();
		
			return services;
		}

		public IServiceCollection AddFileServices()
		{
			services.TryAddSingleton<IFileAppender, DefaultFileAppender>();
			services.TryAddSingleton<IFileMetadata, DefaultFileMetadata>();
			services.TryAddSingleton<IFileOperator, DefaultFileOperator>();
			services.TryAddSingleton<IFileReader, DefaultFileReader>();
			services.TryAddSingleton<IFileSearcher, DefaultFileSearcher>();
			services.TryAddSingleton<IFileStreamer, DefaultFileStreamer>();
			services.TryAddSingleton<IFileWriter, DefaultFileWriter>();
		
			return services;
		}
		
		public IServiceCollection AddPathServices()
		{
			services.TryAddSingleton<IPathCombiner, DefaultPathCombiner>();
			services.TryAddSingleton<IPathGenerator, DefaultPathGenerator>();
			services.TryAddSingleton<IPathParser, DefaultPathParser>();
			services.TryAddSingleton<IPathResolver, DefaultPathResolver>();
		
			return services;
		}
	}
}