namespace Mathematics.Core.Base.Entities;

public abstract class Cipher<T> : Entity
{
	public abstract T Encrypt(T data);

	public abstract T Decrypt(T data);
}