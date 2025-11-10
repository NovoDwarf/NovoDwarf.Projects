using Mathematics.Cryptography.Interfaces;

namespace Mathematics.Cryptography.Models.Monoalphabet;

public class CaesarCipher : ICipher<byte[]>
{
	private readonly int _shift;
	private readonly bool _useModulo;

	public CaesarCipher(int shift, bool useModulo = true)
	{
		this._shift = shift;
		this._useModulo = useModulo;
	}
	
	public byte[] Encrypt(byte[] data)
	{
		var encrypted = new byte[data.Length];
        
		for (var i = 0; i < data.Length; i++)
		{
			encrypted[i] = EncryptByte(data[i]);
		}
        
		return encrypted;
	}

	public byte[] Decrypt(byte[] encryptedData)
	{
		var decrypted = new byte[encryptedData.Length];
        
		for (var i = 0; i < encryptedData.Length; i++)
		{
			decrypted[i] = DecryptByte(encryptedData[i]);
		}
        
		return decrypted;
	}

	private byte EncryptByte(byte b)
	{
		if (_useModulo)
		{
			return (byte)((b + _shift) % 256);
		}

		var result = b + _shift;
		
		return result switch
		{
			> 255 => 255,
			< 0 => 0,
			_ => (byte)result
		};
	}

	private byte DecryptByte(byte b)
	{
		if (_useModulo)
		{
			var result = (b - _shift) % 256;
			
			if (result < 0)
				result += 256;
			
			return (byte)result;
		}
		else
		{
			var result = b - _shift;
			
			return result switch
			{
				> 255 => 255,
				< 0 => 0,
				_ => (byte)result
			};
		}
	}

}