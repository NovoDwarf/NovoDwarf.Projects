namespace Messager.Interfaces.Core;

public interface IBroker<TEvent>
{
	public Guid Id { get; internal set; }
	
	public string BrokerType { get; }
	public string EventType { get; }
}