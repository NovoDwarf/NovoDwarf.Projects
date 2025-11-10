using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;

namespace Messager.Interfaces.Factories;





public interface ISimpleBrokerFactory
{
	ISender<TEvent> GetSender<TEvent>();
	IReceiver<TEvent> GetReceiver<TEvent>();

	IAsyncSender<TEvent> GetAsyncSender<TEvent>();
	IAsyncReceiver<TEvent> GetAsyncReceiver<TEvent>();
}