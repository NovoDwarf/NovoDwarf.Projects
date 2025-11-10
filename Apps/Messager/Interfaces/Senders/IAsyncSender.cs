namespace Messager.Interfaces.Senders;

public interface IAsyncSender<in TEvent>
{
	ValueTask SendAsync(TEvent evt);
}

public interface IAsyncSender<in TKey, in TEvent> where TKey : notnull
{
	ValueTask SendAsync(TKey key, TEvent evt);
}