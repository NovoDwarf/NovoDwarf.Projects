using Komissar.Core.Enums;

namespace Komissar.Runtime;

public sealed class SimOptions
{
    public SimMode Mode { get; set; } = SimMode.Turn;

    public TimeSpan TickInterval { get; set; } = TimeSpan.FromMinutes(1);

    public int MaxDegreeOfParallelism { get; set; } = 1;

    public static SimOptions Turn { get; } = new();

    public static SimOptions Real(TimeSpan tickInterval) => new()
    {
        Mode = SimMode.Real,
        TickInterval = tickInterval
    };
}
