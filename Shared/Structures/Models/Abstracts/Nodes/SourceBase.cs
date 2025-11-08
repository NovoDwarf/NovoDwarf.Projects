using Structures.Models.Abstracts.Commons.Nodes;
using Structures.Models.Abstracts.Options;

namespace Structures.Models.Abstracts.Nodes;

public abstract class SourceBase : DistributionNode
{
	protected readonly SourceOptions Options;

	protected int InFlight;
	protected bool IsBlocked;
	protected double NextTime;

	protected SourceBase(SourceOptions? options = null) : base(options)
	{
		Options = options ?? new SourceOptions();
	}

	protected int ClosedPopulation => Options.ClosedPopulation;

	public int GeneratedRequests { get; protected set; }

	public abstract void Generate();
}