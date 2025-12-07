using System.Collections.Concurrent;
using Mathematics.App.Maui.Services;
using Mathematics.App.Maui.UI.ViewModels;
using Mathematics.Core.Base.Entities;

namespace Mathematics.App.Maui.Factories;

public class DistributionPreViewModelFactory
{
	private readonly ConcurrentDictionary<Type, DistributionPreViewModel> _cache = new();
	private readonly IServiceProvider _serviceProvider;
	private readonly Lock _lock = new();
    
	public DistributionPreViewModelFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}
    
	public DistributionPreViewModel GetOrCreate(Distribution distribution)
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
    
	private DistributionPreViewModel CreateViewModel(Distribution distribution)
	{
		return new DistributionPreViewModel(
			distribution, 
			_serviceProvider.GetRequiredService<NavigationService>(), 
			_serviceProvider.GetRequiredService<DistributionPageFactory>());
	}
}