using Mathematics.Cryptography.Monoalphabet;

namespace Mathematics.Tests.Models.Cryptography;

[TestFixture]
public class CaesarCipherTests
{
	[Test]
	public void Encrypt_WithModulo_ShouldReturnEncryptedData()
	{
		var cipher = new CaesarCipher(3);
		var data = "ABC"u8.ToArray();

		var encryptedData = cipher.Encrypt(data);

		Assert.That("DEF"u8.ToArray(), Is.EqualTo(encryptedData));
	}
}