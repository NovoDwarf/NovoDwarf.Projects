using Mathematics.Core.Base.Entities;

namespace Mathematics.Cryptography.Monoalphabet;

public class SimpleSubstitutionCipher : Cipher<byte[]>
{
	private readonly Dictionary<byte, byte> _decryptionMap;
	private readonly Dictionary<byte, byte> _encryptionMap;

	public SimpleSubstitutionCipher(Dictionary<byte, byte> substitutionMap)
	{
		_encryptionMap = new Dictionary<byte, byte>(substitutionMap);
		_decryptionMap = new Dictionary<byte, byte>();

		foreach (var pair in substitutionMap) _decryptionMap[pair.Value] = pair.Key;
	}

	public override byte[] Encrypt(byte[] data)
	{
		var encrypted = new byte[data.Length];

		for (var i = 0; i < data.Length; i++) encrypted[i] = _encryptionMap.GetValueOrDefault(data[i], data[i]);

		return encrypted;
	}

	public override byte[] Decrypt(byte[] encryptedData)
	{
		var decrypted = new byte[encryptedData.Length];

		for (var i = 0; i < encryptedData.Length; i++)
			decrypted[i] = _decryptionMap.GetValueOrDefault(encryptedData[i], encryptedData[i]);

		return decrypted;
	}
}