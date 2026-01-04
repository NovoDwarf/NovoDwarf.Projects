using System.Diagnostics;
using Modeling.Core.Models.Abstracts.Nodes;
using Modeling.Core.Models.Abstracts.Options;
using Modeling.Core.Models.Base;

namespace Modeling.DeltaT.Algorithm.Models.Nodes;

[DebuggerDisplay("Generator [{Id}]")]
public class Source : SourceBase
{
	private double _blockStartTime;
	private double _lastGenerateTime = -1;

	public Source(SourceOptions? options = null) : base(options)
	{
	}

	public override void OnContextSet()
	{
		base.OnContextSet();
		NextTime = Distribution.Distribute();
	}

	public override void Update(double deltaTime)
	{
		if (Options.ClosedSystem)
			HandleClosedSystem(Context.CurrentTime);
		else
			HandleOpenSystem(Context.CurrentTime);
	}

	public override void Generate()
	{
		var request = Request.Create(Id);
		var next = GetAvailableExit();

		Context.Collector.CounterIncrement($"{Id}_Source_Generated");

		if (Options.ClosedSystem)
			InFlight++;

		if (_lastGenerateTime >= 0)
		{
			var generateTime = Context.CurrentTime - _lastGenerateTime;
			Context.Collector.ListAdd($"{Id}_Source_GenerationTime", generateTime);
		}

		_lastGenerateTime = Context.CurrentTime;

		if (next != null)
			next.Process(request);
		else
			Enqueue(request);
	}

	private void HandleClosedSystem(double currentTime)
	{
		var canGenerate = InFlight < ClosedPopulation;

		if (canGenerate)
		{
			UnblockIfNeeded();
			GenerateWhilePossible(currentTime, () => InFlight < ClosedPopulation);
		}
		else
		{
			BlockIfNeeded();
		}
	}

	private void HandleOpenSystem(double currentTime)
	{
		GenerateWhilePossible(currentTime, () => true);
	}

	private void GenerateWhilePossible(double currentTime, Func<bool> condition)
	{
		while (currentTime >= NextTime && condition())
		{
			Generate();
			NextTime = currentTime + Distribution.Distribute();
		}
	}

	private void BlockIfNeeded()
	{
		if (IsBlocked)
			return;

		IsBlocked = true;
		_blockStartTime = Context.CurrentTime;
	}

	private void UnblockIfNeeded()
	{
		if (!IsBlocked)
			return;

		IsBlocked = false;

		var blockTime = Context.CurrentTime - _blockStartTime;
		Context.Collector.ListAdd($"{Id}_Source_BlockTime", blockTime);
	}
}