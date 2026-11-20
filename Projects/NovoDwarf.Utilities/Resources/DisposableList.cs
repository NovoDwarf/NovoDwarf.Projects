namespace NovoDwarf.Utilities.Resources;

/// <summary>
/// A thread-safe collection of <see cref="IDisposable"/> objects that are disposed
/// together when the list itself is disposed.
/// </summary>
/// <remarks>
/// This class provides a convenient way to manage multiple disposable resources
/// as a single unit. All disposables added to the list will be disposed when
/// the <see cref="Dispose"/> method is called. The class ensures thread-safe
/// operations for both adding items and disposing the entire collection.
/// </remarks>
public sealed class DisposableList : IDisposable
{
    private readonly Lock _lock = new();
    
    private List<IDisposable> _disposables = [];
    private bool _disposed;

    /// <summary>
    /// Adds one or more <see cref="IDisposable"/> objects to the list.
    /// </summary>
    /// <param name="disposables">The disposable objects to add.</param>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the list has already been disposed.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if any of the provided disposables is null.
    /// </exception>
    public void Add(params IDisposable[] disposables)
    {
        ArgumentNullException.ThrowIfNull(disposables);
        
        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _disposables.AddRange(disposables);
        }
    }

    /// <summary>
    /// Gets the number of disposable objects in the list.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _disposables.Count;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the list has been disposed.
    /// </summary>
    public bool IsDisposed
    {
        get
        {
            lock (_lock)
            {
                return _disposed;
            }
        }
    }
    
    /// <summary>
    /// Disposes all <see cref="IDisposable"/> objects in the list and prevents
    /// further modifications. This method is thread-safe and idempotent.
    /// </summary>
    /// <remarks>
    /// Disposable objects are disposed in the order they were added to the list.
    /// If any disposable throws an exception during disposal, it will not prevent
    /// the remaining disposables from being disposed. The first exception encountered
    /// will be re-thrown after all disposables have been processed.
    /// </remarks>
    /// <exception cref="AggregateException"></exception>
    public void Dispose()
    {
        List<IDisposable> toDispose;
        
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;
            toDispose = _disposables;
            _disposables = [];
        }
        
        List<Exception>? exceptions = null;
        
        foreach (var disposable in toDispose)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(disposable);
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                (exceptions ??= []).Add(ex);
            }
        }
        
        if (exceptions is not null)
            throw new AggregateException("One or more disposables failed to dispose.", exceptions);
    }
}
