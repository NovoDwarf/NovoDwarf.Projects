using System.Runtime.Versioning;

namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Encrypts and decrypts files using Windows file system encryption.</summary>
public interface IFileEncryption
{
    /// <inheritdoc cref="File.Encrypt(string)" />
    [SupportedOSPlatform("windows")]
    void Encrypt(string path);

    /// <inheritdoc cref="File.Decrypt(string)" />
    [SupportedOSPlatform("windows")]
    void Decrypt(string path);
}
