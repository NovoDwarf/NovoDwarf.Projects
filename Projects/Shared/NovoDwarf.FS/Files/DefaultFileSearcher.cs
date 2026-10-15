using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileSearcher" />
public sealed class DefaultFileSearcher : IFileSearcher
{
    /// <inheritdoc />
    public string[] GetFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => Directory.GetFiles(path, searchPattern, searchOption);

    /// <inheritdoc />
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => Directory.EnumerateFiles(path, searchPattern, searchOption);
}
