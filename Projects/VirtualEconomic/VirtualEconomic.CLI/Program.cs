using Friflo.Engine.ECS;
using Komissar;
using Komissar.Runtime;
using Komissar.Systems;
using Komissar.Systems.Interfaces;
using Komissar.Utilities;
using Microsoft.Extensions.DependencyInjection;
using VirtualEconomic.Systems;
using VirtualEconomic.Systems.Analytics;
using VirtualEconomic.Systems.Companies;
using VirtualEconomic.Systems.Populations;
using VirtualEconomic.Systems.World;

namespace VirtualEconomic.CLI;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        await using var provider = services.BuildServiceProvider();

        var state = new EconomyState(new EntityStore());
        var systems = provider.GetServices<ISimulationSystem<EconomyState>>().ToList();
        var planner = provider.GetRequiredService<SystemPlanner<EconomyState>>();
        var plan = planner.Build(systems);

        PrintPlan(plan);

        Console.WriteLine();
        Console.WriteLine("=== Executing ===");
        Console.WriteLine();

        var executor = new SimExecutor<EconomyState>(state, new SimClock(), new SimOptions { MaxDegreeOfParallelism = 4 }, TimeProvider.System, []);
        var context = new SimExecContext(Tick: 1, Delta: TimeSpan.FromMinutes(1), Timestamp: DateTimeOffset.UtcNow);
        
        await executor.ExecuteAsync(plan, context, CancellationToken.None);

        Console.WriteLine();
        Console.WriteLine("=== Final State ===");
        Console.WriteLine();

        Console.WriteLine($"Population: {state.Population}");
    }

    private static void ConfigureServices(
        IServiceCollection services)
    {
        services.AddSingleton<TopoSorter>();
        services.AddSingleton<SystemPlanner<EconomyState>>();
    }

    private static void PrintPlan(
        ExecutionPlan<EconomyState> plan)
    {
        Console.WriteLine("=== Simulation Execution Plan ===");
        Console.WriteLine();

        foreach (var wave in plan.Waves)
        {
            Console.WriteLine(
                $"Wave {wave.Index} [{wave.ExecutionMode}]");

            foreach (var system in wave.Systems)
            {
                Console.WriteLine($"  -> {system.GetType().Name}");
            }

            Console.WriteLine();
        }

        Console.WriteLine($"Systems: {plan.SystemCount}");
        Console.WriteLine($"Waves:   {plan.WaveCount}");
    }
}