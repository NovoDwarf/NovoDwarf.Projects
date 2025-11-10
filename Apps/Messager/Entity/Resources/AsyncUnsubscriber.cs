namespace Messager.Entity.Resources;

public sealed class AsyncUnsubscriber : IAsyncDisposable
{
	private readonly Func<ValueTask> _unsubscribe;
	private bool _disposed;

	public AsyncUnsubscriber(Func<ValueTask> unsubscribe)
	{
		_unsubscribe = unsubscribe;
	}

	public async ValueTask DisposeAsync()
	{
		if (_disposed)
			return;

		_disposed = true;
		
		await _unsubscribe();
	}
}