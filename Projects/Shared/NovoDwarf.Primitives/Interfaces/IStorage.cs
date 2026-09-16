namespace NovoDwarf.Primitives.Interfaces;

public interface IStorage<T>
{
	public int Count { get; }
	public bool IsEmpty { get; }

	public void Enqueue(T request);
	public T? Dequeue();
	public T? Peek();
}