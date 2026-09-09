using Komissar;
using Komissar.Executions;
using Komissar.Runtime;
using Komissar.Systems.Interfaces;
using SultanDynasty.Instances.Characters;

namespace SultanDynasty.Simulation.Systems;

public sealed class NeedsSystem : ISimulationSystem<World>
{
    public SystemDescriptor Descriptor { get; } = new(writes: [typeof(Needs)]);

    public ValueTask ExecuteAsync(SimulationContext<World> context, CancellationToken cancellationToken)
    {
        var days = (float)context.Delta.TotalDays;

        foreach (var record in context.State.Characters)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var needs = record.Character.State.Needs;
            needs.Hunger = Math.Clamp(needs.Hunger + days, 0f, 1f);
            needs.Hydration = Math.Clamp(needs.Hydration + days * 1.5f, 0f, 1f);
            needs.Fatigue = Math.Clamp(needs.Fatigue + days * 0.5f, 0f, 1f);
            needs.Energy = Math.Clamp(needs.Energy - days * 0.5f, 0f, 1f);

            context.Commands.Enqueue(_ => record.Character.State.Needs = needs);
        }

        return ValueTask.CompletedTask;
    }
}
