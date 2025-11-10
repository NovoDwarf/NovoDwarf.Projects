using Mathematics.Cryptography.Interfaces;

namespace Mathematics.Cryptography.Models.Monoalphabet;

public class SimpleSubstitutionCipher : ICipher<byte[]>
{
    private readonly Dictionary<byte, byte> encryptionMap;
    private readonly Dictionary<byte, byte> decryptionMap;

    public SimpleSubstitutionCipher(Dictionary<byte, byte> substitutionMap)
    {
        encryptionMap = new Dictionary<byte, byte>(substitutionMap);
        decryptionMap = new Dictionary<byte, byte>();
        
        foreach (var pair in substitutionMap)
        {
            decryptionMap[pair.Value] = pair.Key;
        }
    }

    public byte[] Encrypt(byte[] data)
    {
        var encrypted = new byte[data.Length];
        
        for (var i = 0; i < data.Length; i++)
        {
            encrypted[i] = encryptionMap.GetValueOrDefault(data[i], data[i]);
        }
        
        return encrypted;
    }

    public byte[] Decrypt(byte[] encryptedData)
    {
        var decrypted = new byte[encryptedData.Length];
        
        for (var i = 0; i < encryptedData.Length; i++)
        {
            decrypted[i] = decryptionMap.GetValueOrDefault(encryptedData[i], encryptedData[i]);
        }
        
        return decrypted;
    }
}