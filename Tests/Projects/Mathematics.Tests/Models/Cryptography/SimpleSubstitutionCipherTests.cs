
using Mathematics.Cryptography.Monoalphabet;

namespace Mathematics.Tests.Models.Cryptography;

public class SimpleSubstitutionCipherTests
{
	[Test]
	public void Encrypt_WithModulo_ShouldReturnEncryptedData()
	{
		var map = new Dictionary<byte, byte>
		{
			{ (byte)'A', (byte)'D' },
			{ (byte)'B', (byte)'E' },
			{ (byte)'C', (byte)'F' }
		};
		var data = "ABC"u8.ToArray();
		var cipher = new SimpleSubstitutionCipher(map);

		var encryptedData = cipher.Encrypt(data);

		Assert.That("DEF"u8.ToArray(), Is.EqualTo(encryptedData));
	}
}