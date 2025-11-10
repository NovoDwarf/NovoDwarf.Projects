namespace Messager.Interfaces.Receivers;

public interface IAsyncReceiver<out TEvent>
{
	IAsyncDisposable Subscribe(Func<TEvent, ValueTask> handler);
	void Unsubscribe(Func<TEvent, ValueTask> handler);
}

public interface IAsyncReceiver<in TKey, out TEvent> where TKey : notnull
{
	IAsyncDisposable Subscribe(TKey key, Func<TEvent, ValueTask> handler);
	void Unsubscribe(TKey key, Func<TEvent, ValueTask> handler);
}