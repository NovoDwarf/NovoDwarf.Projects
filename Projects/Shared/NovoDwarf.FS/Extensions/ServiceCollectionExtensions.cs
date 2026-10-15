using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NovoDwarf.FS.Directories;
using NovoDwarf.FS.Directories.Interfaces;
using NovoDwarf.FS.Files;
using NovoDwarf.FS.Files.Interfaces;
using NovoDwarf.FS.Links;
using NovoDwarf.FS.Paths;
using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Extensions;

/// <summary>Registers the local file system adapters without replacing existing registrations.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers symbolic link operations for files and directories.</summary>
        public IServiceCollection AddSymbolicLinkServices()
        {
            services.TryAddSingleton<ISymbolicLinkService, DefaultSymbolicLinkService>();

            return services;
        }

        /// <summary>Registers the corresponding local file system services as singletons.</summary>
        public IServiceCollection AddFileSystemServices()
        {
            services.AddDirectoryServices();
            services.AddFileServices();
            services.AddPathServices();
            services.AddSymbolicLinkServices();

            return services;
        }

        /// <summary>Registers the corresponding local file system services as singletons.</summary>
        public IServiceCollection AddDirectoryServices()
        {
            services.TryAddSingleton<IDirectoryMetadata, DefaultDirectoryMetadata>();
            services.TryAddSingleton<IDirectoryReader, DefaultDirectoryReader>();
            services.TryAddSingleton<IDirectoryWriter, DefaultDirectoryWriter>();

            return services;
        }

        /// <summary>Registers the corresponding local file system services as singletons.</summary>
        public IServiceCollection AddFileServices()
        {
            services.TryAddSingleton<IFileEncryption, DefaultFileEncryption>();
            services.TryAddSingleton<IFileAppender, DefaultFileAppender>();
            services.TryAddSingleton<IFileMetadata, DefaultFileMetadata>();
            services.TryAddSingleton<IFileOperator, DefaultFileOperator>();
            services.TryAddSingleton<IFileReader, DefaultFileReader>();
            services.TryAddSingleton<IFileSearcher, DefaultFileSearcher>();
            services.TryAddSingleton<IFileStreamer, DefaultFileStreamer>();
            services.TryAddSingleton<IFileWriter, DefaultFileWriter>();

            return services;
        }

        /// <summary>Registers the corresponding local file system services as singletons.</summary>
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
