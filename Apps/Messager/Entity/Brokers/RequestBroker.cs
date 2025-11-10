using Messager.Interfaces.Requests;

namespace Messager.Entity.Brokers;

public class RequestBroker<TOut, TIn> : IRequest<TOut, TIn>
{
	private readonly IRequest<TOut, TIn> _impl;

	public RequestBroker(IRequest<TOut, TIn> impl)
	{
		_impl = impl;
	}

	public TOut Request(TIn input) => _impl.Request(input);
}

public class RequestBroker<TKey, TOut, TIn> : IRequest<TKey, TOut, TIn> where TKey : notnull
{
	private readonly IRequest<TKey, TOut, TIn> _impl;
    
	public RequestBroker(IRequest<TKey, TOut, TIn> impl)
	{
		_impl = impl;
	}
	
	public TOut Request(TKey key, TIn input) => _impl.Request(key, input);
}