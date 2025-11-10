using Messager.Interfaces.Factories;
using Messager.Interfaces.Receivers;

namespace Messager.Entity.Receivers;

public class Receiver<TEvent> : IReceiver<TEvent>
{
	private readonly IReceiver<TEvent> _impl;
	
	public Receiver(ISimpleBrokerFactory factory)
	{
		_impl = factory.GetReceiver<TEvent>();
	}

	public IDisposable Subscribe(Action<TEvent> handler) => _impl.Subscribe(handler);
}