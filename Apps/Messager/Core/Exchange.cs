using Messager.Entity.Registers;
using Messager.Interfaces.Factories;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Microsoft.Extensions.Logging;

namespace Messager.Core;

public sealed class Exchange : ISimpleBrokerFactory, IKeyedMessageBrokerFactory
{
    private readonly SimpleBrokerRegistry _simpleBrokers;
    private readonly KeyedBrokerRegistry _keyedBrokers;
    private readonly AsyncSimpleBrokerRegistry _asyncSimpleBrokers;
    private readonly AsyncKeyedBrokerRegistry _asyncKeyedBrokers;
    
    public Exchange(ILoggerFactory? loggerFactory = null)
    {
        _simpleBrokers = new SimpleBrokerRegistry(loggerFactory);
        _keyedBrokers = new KeyedBrokerRegistry(loggerFactory);
        _asyncSimpleBrokers = new AsyncSimpleBrokerRegistry(loggerFactory);
        _asyncKeyedBrokers = new AsyncKeyedBrokerRegistry(loggerFactory);
    }
    
    public ISender<TEvent> GetSender<TEvent>() => _simpleBrokers.GetOrCreate<TEvent>();
    public IReceiver<TEvent> GetReceiver<TEvent>() => _simpleBrokers.GetOrCreate<TEvent>();
    
    public IAsyncSender<TEvent> GetAsyncSender<TEvent>() => _asyncSimpleBrokers.GetOrCreate<TEvent>();
    public IAsyncReceiver<TEvent> GetAsyncReceiver<TEvent>() => _asyncSimpleBrokers.GetOrCreate<TEvent>();
    
    public ISender<TKey, TEvent> GetKeyedSender<TKey, TEvent>() where TKey : notnull => _keyedBrokers.GetOrCreate<TKey, TEvent>();
    public IReceiver<TKey, TEvent> GetKeyedReceiver<TKey, TEvent>() where TKey : notnull => _keyedBrokers.GetOrCreate<TKey, TEvent>();
    
    public IAsyncSender<TKey, TEvent> GetAsyncKeyedSender<TKey, TEvent>() where TKey : notnull => _asyncKeyedBrokers.GetOrCreate<TKey, TEvent>();
    public IAsyncReceiver<TKey, TEvent> GetAsyncKeyedReceiver<TKey, TEvent>() where TKey : notnull => _asyncKeyedBrokers.GetOrCreate<TKey, TEvent>();
}