using NovoDwarf.Primitives.Interfaces;

namespace NovoDwarf.Primitives.Models.Storages;

public class FifoStorage<T> : IStorage<T>
{
	private readonly Queue<T> _queue = new();

	public int Count => _queue.Count;
	public bool IsEmpty => _queue.Count == 0;

	public void Enqueue(T request)
	{
		_queue.Enqueue(request);
	}

	public T Dequeue()
	{
		return _queue.Dequeue();
	}

	public T Peek()
	{
		return _queue.Peek();
	}
}