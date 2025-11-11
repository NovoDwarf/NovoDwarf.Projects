namespace Messager.Entity.Resources;

public sealed class DisposableList : IDisposable
{
	private readonly List<IDisposable> _disposables = [];

	private bool _disposed;

	public void Add(params IDisposable[] disposables)
	{
		_disposables.AddRange(disposables);
	}

	public void Dispose()
	{
		ObjectDisposedException.ThrowIf(_disposed, typeof(DisposableList));
		
		foreach (var disposable in _disposables) 
			disposable.Dispose();
		
		_disposed = true;
		_disposables.Clear();
	}
}