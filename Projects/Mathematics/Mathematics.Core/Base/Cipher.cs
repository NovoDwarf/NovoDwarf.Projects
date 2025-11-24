namespace Mathematics.Core.Base;

public abstract class Cipher<T> : Entity
{
	public override string Category => "cipher_category";
	
	public abstract T Encrypt(T data);

	public abstract T Decrypt(T data);
}