namespace Messager.Interfaces.Requests;

public interface IAsyncRequest<TOut, TIn>
{
	ValueTask<TOut> RequestAsync(TIn input);
}

public interface IAsyncRequest<TKey, TOut, TIn>
{
	TKey Key { get; }
	
	ValueTask<TOut> RequestAsync(TKey key, TIn input);
}