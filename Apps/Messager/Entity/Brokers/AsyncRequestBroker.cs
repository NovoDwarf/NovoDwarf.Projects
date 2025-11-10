using Messager.Interfaces.Requests;

namespace Messager.Entity.Brokers;

public class AsyncRequestBroker<TOut, TIn> : IAsyncRequest<TOut, TIn>
{
	private readonly IAsyncRequest<TOut, TIn> _impl;

	public AsyncRequestBroker(IAsyncRequest<TOut, TIn> impl)
	{
		_impl = impl;
	}

	public ValueTask<TOut> RequestAsync(TIn input) => _impl.RequestAsync(input);
    
	//public bool TryRequest(TIn input, out TOut? output) => _impl.TryRequest(input, out output);
}

public class AsyncRequestBroker<TKey, TOut, TIn> : IAsyncRequest<TKey, TOut, TIn>
{
	private readonly IAsyncRequest<TKey, TOut, TIn> _impl;

	public AsyncRequestBroker(IAsyncRequest<TKey, TOut, TIn> impl)
	{
		_impl = impl;
	}

	public TKey Key => _impl.Key;
	
	public ValueTask<TOut> RequestAsync(TKey key, TIn input) => _impl.RequestAsync(key, input);
    
	//public bool TryRequest(TIn input, out TOut? output) => _impl.TryRequest(input, out output);
}