namespace Mathematics.Core.Cryptography.Interfaces;

public interface ICipher<T>
{
	public T Encrypt(T data);
	public T Decrypt(T data);
}