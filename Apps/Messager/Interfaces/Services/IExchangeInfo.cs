namespace Messager.Interfaces.Services;

public interface IExchangeInfo
{
	List<(string EventName, int SubscriberCount)> GetSimpleStats();
	List<(string EventName, int SubscriberCount)> GetKeyedStats();
	List<(string EventName, int SubscriberCount)> GetAsyncSimpleStats();
	List<(string EventName, int SubscriberCount)> GetAsyncKeyedStats();
	List<(string EventName, int SubscriberCount)> GetRequestStats();
	List<(string EventName, int SubscriberCount)> GetAsyncRequestStats();
}