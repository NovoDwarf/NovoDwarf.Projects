using Komissar.Core.Enums;
using Komissar.Extensions;
using Komissar.Runtime;
using Komissar.Systems;
using Komissar.Systems.Samples;

var services = new ServiceCollection();

// Setup Komissar
services.AddKomissar<long>(opts =>
{
    opts.Mode = SimMode.Turn;
    opts.TickInterval = TimeSpan.FromSeconds(2);
    opts.MaxDegreeOfParallelism = 4;
});

// Register systems
services
    .AddSimulationSystems<long>(
        sp => sp.GetRequiredService<InitializerSystem>(),
        sp => sp.GetRequiredService<IncrementorSystem>(),
        sp => sp.GetRequiredService<LoggerSystem>(TimeSpan.FromSeconds(5)),
        singleton: true)
    .AddSimulationSystems<long>(
        sp => sp.GetRequiredService<FinalizerSystem>(),
        singleton: true);

var provider = services.BuildServiceProvider();

var runtime = provider.GetRequiredService<ISimRuntime>();

Console.WriteLine("Starting simulation...");
await runtime.StartAsync();

// Run for 3 ticks manually
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"\n--- Tick {i + 1} --");
    await runtime.StepAsync();
}

Console.WriteLine("\nPausing...");
await runtime.PauseAsync();

Console.WriteLine("\nWaiting 5 seconds in real time...");
await Task.Delay(TimeSpan.FromSeconds(5));

Console.WriteLine("\nResuming...");
await runtime.StepAsync();

Console.WriteLine("\nStopping...");
await runtime.StopAsync();

Console.WriteLine("\nSimulation complete.");
