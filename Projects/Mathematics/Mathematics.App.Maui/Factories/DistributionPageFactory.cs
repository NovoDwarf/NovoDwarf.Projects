using System.Collections.Concurrent;
using Mathematics.App.Maui.UI.ViewModels;
using Mathematics.App.Maui.UI.Views.Distributions;
using Mathematics.Core.Base.Entities;
using Microsoft.Extensions.Logging;

namespace Mathematics.App.Maui.Factories;

public class DistributionPageFactory
{
	private readonly ConcurrentDictionary<Type, DistributionView> _cache = new();
	private readonly IServiceProvider _serviceProvider;
	private readonly Lock _lock = new();
    
	public DistributionPageFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}
    
	public DistributionView GetOrCreate(Distribution distribution)
	{
		var distributionType = distribution.GetType();
        
		lock (_lock)
		{
			if (_cache.TryGetValue(distributionType, out var value)) 
				return value;
			
			value = CreateViewModel(distribution);
			_cache[distributionType] = value;

			return value;
		}
	}
    
	private DistributionView CreateViewModel(Distribution distribution)
	{
		return new DistributionView(distribution, _serviceProvider.GetRequiredService<ILogger<DistributionViewModel>>());
	}
}