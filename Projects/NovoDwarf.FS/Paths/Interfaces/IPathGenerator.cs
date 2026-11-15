namespace NovoDwarf.FS.Paths.Interfaces;

/// <summary>Generates temporary paths and random names. GetTempFileName also creates an empty file.</summary>
public interface IPathGenerator
{
    /// <inheritdoc cref="Path.GetRandomFileName()" />
    public string GetRandomFileName();
    
    /// <inheritdoc cref="Path.GetTempFileName()" />
    public string GetTempFileName();

    /// <inheritdoc cref="Path.GetTempPath()" />
    public string GetTempPath();
}
