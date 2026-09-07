using Komissar.Core.Enums;

namespace Komissar;

public sealed class SimulationOptions
{
    public SimulationMode Mode { get; set; } = SimulationMode.Turn;

    public TimeSpan TickInterval { get; set; } = TimeSpan.FromMinutes(1);

    public int MaxDegreeOfParallelism { get; set; } = 1;

    public static SimulationOptions Turn { get; } = new();

    public static SimulationOptions Real(TimeSpan tickInterval) => new()
    {
        Mode = SimulationMode.Real,
        TickInterval = tickInterval
    };
}
