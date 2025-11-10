namespace Messager.Interfaces.Senders;

public interface ISender<in TEvent>
{
	void Send(TEvent evt);
}

public interface ISender<in TKey, in TEvent> where TKey : notnull
{
	void Send(TKey key, TEvent evt);
}