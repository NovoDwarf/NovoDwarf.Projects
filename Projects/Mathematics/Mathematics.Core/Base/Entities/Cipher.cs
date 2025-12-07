using Mathematics.Core.Base.Entities;

namespace Mathematics.Core.Base;

public abstract class Cipher<T> : Entity
{
	public override string[] Path { get; }

	public abstract T Encrypt(T data);

	public abstract T Decrypt(T data);
}