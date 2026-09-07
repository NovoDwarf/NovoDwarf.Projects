using Komissar.Core.Interfaces;
using Komissar.Diagnostics;
using Komissar.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Komissar.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKomissar<TState>(
        this IServiceCollection services,
        Func<IServiceProvider, TState> stateFactory,
        Action<SimulationOptions>? configure = null,
        Action<TimeScale>? configureTimeScale = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(stateFactory);

        var options = services.AddOptions<SimulationOptions>();

        if (configure is not null)
            options.Configure(configure);

        services.TryAddSingleton<Clock>();
        services.TryAddSingleton(_ => CreateTimeScale(configureTimeScale));
        services.TryAddSingleton<MetricsCollector<TState>>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<ISimulationObserver<TState>>(static provider => provider.GetRequiredService<MetricsCollector<TState>>()));
        services.TryAddSingleton<SimulationRuntime<TState>>(provider => new SimulationRuntime<TState>(
            stateFactory(provider),
            provider.GetRequiredService<IOptions<SimulationOptions>>(),
            provider.GetServices<ISimulationSystem<TState>>(),
            clock: provider.GetRequiredService<Clock>(),
            timeScale: provider.GetRequiredService<TimeScale>(),
            observers: provider.GetServices<ISimulationObserver<TState>>()));
        services.TryAddSingleton<ISimulationRuntime>(static provider => provider.GetRequiredService<SimulationRuntime<TState>>());

        return services;
    }

    public static IServiceCollection AddKomissarSystem<TState, TSystem>(this IServiceCollection services)
        where TSystem : class, ISimulationSystem<TState>
    {
        services.AddSingleton<ISimulationSystem<TState>, TSystem>();
        return services;
    }

    private static TimeScale CreateTimeScale(Action<TimeScale>? configure)
    {
        var timeScale = new TimeScale();
        configure?.Invoke(timeScale);
        
        return timeScale;
    }
}
