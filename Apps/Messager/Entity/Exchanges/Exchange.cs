using Messager.Entity.Helpers;
using Messager.Entity.Registers;
using Messager.Interfaces.Factories;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;

namespace Messager.Entity.Exchanges;

public sealed class Exchange : ISimpleBrokerFactory, IKeyedMessageBrokerFactory
{
    private readonly SimpleBrokerRegistry _simpleBrokers;
    private readonly KeyedBrokerRegistry _keyedBrokers;
    private readonly AsyncSimpleBrokerRegistry _asyncSimpleBrokers;
    private readonly AsyncKeyedBrokerRegistry _asyncKeyedBrokers;
    
    public Exchange()
    {
        _simpleBrokers = new SimpleBrokerRegistry();
        _keyedBrokers = new KeyedBrokerRegistry();
        _asyncSimpleBrokers = new AsyncSimpleBrokerRegistry();
        _asyncKeyedBrokers = new AsyncKeyedBrokerRegistry();
        
        Monitor = new ExchangeMonitor(_simpleBrokers, _keyedBrokers, _asyncSimpleBrokers, _asyncKeyedBrokers);
    }
    
    public ExchangeMonitor Monitor { get; private set; }
    
    public ISender<TEvent> GetSender<TEvent>() => _simpleBrokers.GetOrCreate<TEvent>();
    public IReceiver<TEvent> GetReceiver<TEvent>() => _simpleBrokers.GetOrCreate<TEvent>();
    
    public IAsyncSender<TEvent> GetAsyncSender<TEvent>() => _asyncSimpleBrokers.GetOrCreate<TEvent>();
    public IAsyncReceiver<TEvent> GetAsyncReceiver<TEvent>() => _asyncSimpleBrokers.GetOrCreate<TEvent>();
    
    public ISender<TKey, TEvent> GetKeyedSender<TKey, TEvent>() where TKey : notnull => _keyedBrokers.GetOrCreate<TKey, TEvent>();
    public IReceiver<TKey, TEvent> GetKeyedReceiver<TKey, TEvent>() where TKey : notnull => _keyedBrokers.GetOrCreate<TKey, TEvent>();
    
    public IAsyncSender<TKey, TEvent> GetAsyncKeyedSender<TKey, TEvent>() where TKey : notnull => _asyncKeyedBrokers.GetOrCreate<TKey, TEvent>();
    public IAsyncReceiver<TKey, TEvent> GetAsyncKeyedReceiver<TKey, TEvent>() where TKey : notnull => _asyncKeyedBrokers.GetOrCreate<TKey, TEvent>();
}