namespace Messager.Interfaces.Requests;

public interface IRequest<TOutput, in TInput>
{
	public TOutput Request(TInput input);
}

public interface IRequest<TKey, TOut, in TIn> where TKey : notnull
{
	public TOut Request(TKey key, TIn input);
}

