using Mathematics.Core.Enums.Cryptography;

namespace Mathematics.Core.Base.Entities;

public abstract class Sorting<T> : Entity
	where T : IComparable<T>
{
	public Action<T[]>? OnStep { get; set; }
	
	public abstract void Sort(in T[] array);
}