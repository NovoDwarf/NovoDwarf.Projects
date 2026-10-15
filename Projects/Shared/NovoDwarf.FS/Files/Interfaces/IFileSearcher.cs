namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Finds files within a directory.</summary>
public interface IFileSearcher
{
    /// <inheritdoc cref="Directory.GetFiles(string, string, SearchOption)" />
    public string[] GetFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <inheritdoc cref="Directory.EnumerateFiles(string, string, SearchOption)" />
    public IEnumerable<string> EnumerateFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);
}
