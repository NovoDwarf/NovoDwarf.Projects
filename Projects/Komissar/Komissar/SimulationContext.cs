using Komissar.Executions;
using Komissar.Time;

namespace Komissar;

public sealed class SimulationContext<TState>
{
    public SimulationContext(TState state, long tick, Clock clock, TimeSpan delta, DateTimeOffset timestamp, CommandBuffer<TState> commands)
    {
        State = state;
        Tick = tick;
        Clock = clock;
        Delta = delta;
        Timestamp = timestamp;
        Commands = commands;
    }

    public TState State { get; }

    public long Tick { get; }

    public Clock Clock { get; }
    public TimeSpan Delta { get; }
    public DateTimeOffset Timestamp { get; }
    public CommandBuffer<TState> Commands { get; }
}
