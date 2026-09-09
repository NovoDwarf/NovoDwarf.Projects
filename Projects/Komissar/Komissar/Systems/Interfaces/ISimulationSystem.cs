using Komissar.Core.Enums;
using Komissar.Runtime;

namespace Komissar.Systems.Interfaces;

public interface ISimulationSystem<TState>
{
    public SimExecMode ExecutionMode { get; }
    
    public ValueTask ExecuteAsync(SimContext<TState> context, CancellationToken cancellationToken);
}