using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;

namespace Messager.Interfaces.Factories;

public interface IKeyedMessageBrokerFactory
{
	ISender<TKey, TEvent> GetKeyedSender<TKey, TEvent>() where TKey : notnull;
	IReceiver<TKey, TEvent> GetKeyedReceiver<TKey, TEvent>() where TKey : notnull;

	IAsyncSender<TKey, TEvent> GetAsyncKeyedSender<TKey, TEvent>() where TKey : notnull;
	IAsyncReceiver<TKey, TEvent> GetAsyncKeyedReceiver<TKey, TEvent>() where TKey : notnull;
}