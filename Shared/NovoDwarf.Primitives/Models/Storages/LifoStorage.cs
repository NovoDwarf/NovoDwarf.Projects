using NovoDwarf.Primitives.Interfaces;

namespace NovoDwarf.Primitives.Models.Storages;

public class LifoStorage<T> : IStorage<T>
{
	private readonly Stack<T> _stack = new();

	public int Count => _stack.Count;
	public bool IsEmpty => _stack.Count == 0;

	public void Enqueue(T request)
	{
		_stack.Push(request);
	}

	public T Dequeue()
	{
		return _stack.Pop();
	}

	public T Peek()
	{
		return _stack.Peek();
	}
}