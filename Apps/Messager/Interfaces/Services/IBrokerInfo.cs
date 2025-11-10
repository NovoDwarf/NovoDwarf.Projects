namespace Messager.Interfaces.Services;

public interface IBrokerInfo
{
	public int SubscriberCount { get; }
	public bool IsEmpty();
}