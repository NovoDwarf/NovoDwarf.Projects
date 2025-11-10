namespace Messager.Interfaces.Receivers;

public interface IReceiver<out TEvent>
{
	IDisposable Subscribe(Action<TEvent> handler);
}

public interface IReceiver<in TKey, out TEvent>
{
	IDisposable Subscribe(TKey key, Action<TEvent> handler);
}