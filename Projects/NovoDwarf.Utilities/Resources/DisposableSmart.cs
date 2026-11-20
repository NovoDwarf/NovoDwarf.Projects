namespace NovoDwarf.Utilities.Resources;

/// <summary>
/// A thread-safe wrapper that ensures a disposable object is disposed exactly once,
/// even if <see cref="Dispose"/> is called multiple times.
/// </summary>
/// <remarks>
/// This class provides a safe way to handle disposable resources by implementing
/// the "disposed" pattern and preventing duplicate disposal attempts. It wraps
/// an existing <see cref="IDisposable"/> instance and guarantees that the underlying
/// disposable is only disposed once, regardless of how many times the wrapper's
/// <see cref="Dispose"/> method is invoked.
/// </remarks>
public sealed class DisposableSmart : IDisposable
{
	private readonly IDisposable _disposable;

	/// <summary>
	/// Initializes a new instance of the <see cref="DisposableSmart"/> class that wraps the specified disposable object.
	/// </summary>
	/// <param name="disposable">
	/// The underlying <see cref="IDisposable"/> object to manage. This instance will be disposed when <see cref="Dispose"/> is first called.
	/// </param>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="disposable"/> is null.
	/// </exception>
	public DisposableSmart(IDisposable disposable)
	{
		ArgumentNullException.ThrowIfNull(disposable);
		
		_disposable = disposable;
	}

	/// <summary>
	/// Gets a value indicating whether the object has been disposed.
	/// </summary>
	public bool IsDisposed => _disposed != 0;
	
	private int _disposed;
    
	/// <summary>
	/// Disposes the underlying <see cref="IDisposable"/> object if it hasn't
	/// been disposed already. Subsequent calls to this method will have no effect.
	/// </summary>
	/// <remarks>
	/// This method is thread-safe and idempotent. The first call to this method
	/// will dispose the wrapped disposable object, and any subsequent calls will
	/// be ignored. This prevents duplicate disposal attempts which could lead to
	/// exceptions or other unexpected behavior.
	/// </remarks>
	public void Dispose()
	{
		if (Interlocked.Exchange(ref _disposed, 1) == 0)
			_disposable.Dispose();
	}
}