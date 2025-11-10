namespace Messager.Entity.Resources;

public sealed class SmartDisposable : IDisposable
{
	private bool _disposed;
	
	private IDisposable _disposable;

	public SmartDisposable(IDisposable disposable)
	{
		_disposable = disposable;
	}

	public void Dispose()
	{
		if (_disposed)
			return;
		
		_disposable.Dispose();
		_disposed = true;
	}
}