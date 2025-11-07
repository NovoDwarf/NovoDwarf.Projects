using Structures.EX;
using Structures.Interfaces;
using Structures.Models.Abstracts.Commons.Options;
using Structures.Models.Base;

namespace Structures.Models.Abstracts.Commons.Nodes;

/// <summary>
/// Base class for all nodes.
/// </summary>
public abstract class NodeBase : INode
{
    private readonly NodeOptions _options;

    protected NodeBase(NodeOptions? options = null)
    {
        _options = options ?? new NodeOptions();
    }
    
    /// <inheritdoc cref="INode.Id"/>
    public Guid Id { get; } = Guid.NewGuid();

    protected SimulationContext Context { get; set; } = null!;
    
    public void SetContext(SimulationContext context)
    {
        Context = context;
        
        OnContextSet();
    }
    
    public abstract void Process(Request request);

    public abstract void Update(double deltaTime);

    public virtual void OnContextSet() {}
}