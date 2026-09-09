using Komissar.Diagnostics;
using Komissar.Diagnostics.Interfaces;
using Komissar.Runtime;
using Komissar.Runtime.Interfaces;
using Komissar.Systems;
using Komissar.Systems.Interfaces;
using Komissar.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Komissar.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKomissar<TState>(this IServiceCollection services, Action<SimOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new SimOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        services.TryAddSingleton<TimeScale>();
        services.TryAddSingleton<SimClock>();

        services.TryAddSingleton<SimRuntime<TState>>();
        services.TryAddSingleton<ISimRuntime>(sp => sp.GetRequiredService<SimRuntime<TState>>());

        services.TryAddSingleton<SimExecutor<TState>>();
        services.TryAddSingleton<SystemPlanner<TState>>();
        services.TryAddSingleton<TopoSorter>();

        services.TryAddSingleton<ISimObserver<TState>, MetricsCollector<TState>>();

        return services;
    }

    public static IServiceCollection AddSimulationSystem<TSystem, TState>(this IServiceCollection services) where TSystem : class, ISimulationSystem<TState>
    {
        services.AddTransient<ISimulationSystem<TState>, TSystem>();

        return services;
    }
}
