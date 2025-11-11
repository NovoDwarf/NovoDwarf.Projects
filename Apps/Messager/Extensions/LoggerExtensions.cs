using Microsoft.Extensions.Logging;

namespace Messager.Extensions;

public static partial class LoggerExtensions
{
	#region Common

	[LoggerMessage(LogLevel.Trace, "[{BrokerType}<{EventType}> [{Id}]] Sending event to [{Count}] subscribers")]
	public static partial void LogSendingEvent(this ILogger logger, string BrokerType, string EventType, Guid Id, int Count);

	[LoggerMessage(LogLevel.Error, "[{BrokerType}<{EventType}>] [{Id}]] Error invoking handler for event")]
	public static partial void LogErrorInvokingHandler(this ILogger logger, Exception ex, string BrokerType, string EventType, Guid Id);
	
	[LoggerMessage(LogLevel.Debug, "[{BrokerType}<{EventType}> [{Id}]] Subscriber added")]
	public static partial void LogSubscriberAdded(this ILogger logger, string BrokerType, string EventType, Guid Id);
	
	[LoggerMessage(LogLevel.Debug, "[{BrokerType}<{EventType}> [{Id}]] Subscriber removed")]
	public static partial void LogSubscriberRemoved(this ILogger logger, string BrokerType, string EventType, Guid Id);
	
	#endregion
	
	#region Simple Broker 
	
	[LoggerMessage(LogLevel.Debug, "[{BrokerType}<{EventType}> [{Id}]] Removed [{Count}] dead subscribers")]
	public static partial void LogRemovedDeadSubscribers(this ILogger logger, string BrokerType, string EventType, Guid Id, int Count);
	
	[LoggerMessage(LogLevel.Debug, "Unsubscribed {Count} handler(s) from [SimpleBroker<{EventType}>]; Remaining subscribers: [{Counts}]")]
	public static partial void LogUnsubscribedHandlersFromSimpleBroker(this ILogger logger, int Count, string EventType, int Counts);
	
	#endregion
}