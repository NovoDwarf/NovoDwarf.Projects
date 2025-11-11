using Microsoft.Extensions.Logging;

namespace Messager.Extensions;

public static partial class LoggerExtensions
{
	#region Common

	[LoggerMessage(LogLevel.Trace, "[{BrokerType}<{EventType}> [{Id}]] Sending event to [{Count}] subscribers")]
	public static partial void LogSendingEvent(this ILogger logger, string BrokerType, string EventType, Guid Id, int Count);

	[LoggerMessage(LogLevel.Error, "[{BrokerType}<{EventType}>] [{Id}]] Error invoking handler for event")]
	public static partial void LogErrorInvokingHandler(this ILogger logger, Exception ex, string BrokerType, string EventType, Guid Id);
	
	[LoggerMessage(LogLevel.Debug, "[{BrokerType}<{EventType}> [{Id}]] Subscriber added to broker [Total: {Count}]")]
	public static partial void LogSubscriberAdded(this ILogger logger, string BrokerType, string EventType, Guid Id, int Count);
	
	#endregion
	
	#region Simple Broker 
	
	[LoggerMessage(LogLevel.Debug, "Removed [{Count}] dead subscribers from [SimpleBroker<{EventType}>]")]
	public static partial void LogRemovedDeadSubscribersFromSimpleBroker(this ILogger logger, int Count, string EventType);
	
	
	[LoggerMessage(LogLevel.Debug, "Subscriber removed from [SimpleBroker<{EventType}>]; Remaining subscribers: [{Count}]")]
	public static partial void LogSubscriberRemovedFromSimpleBroker(this ILogger logger, string EventType, int Count);

	[LoggerMessage(LogLevel.Debug, "Unsubscribed {Count} handler(s) from [SimpleBroker<{EventType}>]; Remaining subscribers: [{Counts}]")]
	public static partial void LogUnsubscribedHandlersFromSimpleBroker(this ILogger logger, int Count, string EventType, int Counts);
	
	#endregion
}