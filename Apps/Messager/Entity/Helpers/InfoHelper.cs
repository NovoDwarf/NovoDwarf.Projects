using Messager.Entity.Registers;

namespace Messager.Entity.Helpers;

public class ExchangeMonitor
{
	private readonly SimpleBrokerRegistry _simpleBrokers;
	private readonly KeyedBrokerRegistry _keyedBrokers;
	private readonly AsyncSimpleBrokerRegistry _asyncSimpleBrokers;
	private readonly AsyncKeyedBrokerRegistry _asyncKeyedBrokers;
	
	internal ExchangeMonitor(SimpleBrokerRegistry simpleBrokers, KeyedBrokerRegistry keyedBrokers, AsyncSimpleBrokerRegistry asyncSimpleBrokers, AsyncKeyedBrokerRegistry asyncKeyedBrokers)
	{
		_simpleBrokers = simpleBrokers;
		_keyedBrokers = keyedBrokers;
		_asyncSimpleBrokers = asyncSimpleBrokers;
		_asyncKeyedBrokers = asyncKeyedBrokers;
	}

	public List<(string, int)> GetSimpleStats() => _simpleBrokers.GetStats();
	public List<(string, int)> GetKeyedStats() => _keyedBrokers.GetStats();
	public List<(string, int)> GetAsyncSimpleStats() => _asyncSimpleBrokers.GetStats();
	public List<(string, int)> GetAsyncKeyedStats() => _asyncKeyedBrokers.GetStats();
}