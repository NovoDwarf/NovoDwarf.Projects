using System.Runtime.Versioning;
using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileEncryption" />
public sealed class DefaultFileEncryption : IFileEncryption
{
    /// <inheritdoc />
    [SupportedOSPlatform("windows")]
    public void Encrypt(string path)
    {
        File.Encrypt(path);
    }

    /// <inheritdoc />
    [SupportedOSPlatform("windows")]
    public void Decrypt(string path)
    {
        File.Decrypt(path);
    }
}
