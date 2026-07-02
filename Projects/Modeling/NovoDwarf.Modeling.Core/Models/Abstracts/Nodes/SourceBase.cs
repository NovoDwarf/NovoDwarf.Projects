using Modeling.Core.Models.Abstracts.Commons.Nodes;
using Modeling.Core.Models.Abstracts.Options;

namespace Modeling.Core.Models.Abstracts.Nodes;

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

	public double NextEventTime => NextTime;

	public abstract void Generate();

	/// <summary>
	/// Уведомление о завершении заявки (для закрытых систем уменьшает InFlight).
	/// </summary>
	public virtual void OnRequestCompleted()
	{
		if (!Options.ClosedSystem)
			return;

		if (InFlight > 0)
			InFlight--;
	}
}