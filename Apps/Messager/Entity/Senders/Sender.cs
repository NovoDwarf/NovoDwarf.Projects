using Messager.Interfaces.Factories;
using Messager.Interfaces.Senders;

namespace Messager.Entity.Senders;

public class Sender<TEvent> : ISender<TEvent>
{
	private readonly ISender<TEvent> _impl;
	
	public Sender(ISimpleBrokerFactory factory)
	{
		_impl = factory.GetSender<TEvent>();
	}

	public void Send(TEvent evt) => _impl.Send(evt);
}