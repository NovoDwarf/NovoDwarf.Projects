using System.Diagnostics;
using Modeling.Core.Models.Abstracts.Nodes;
using Modeling.Core.Models.Abstracts.Options;
using Modeling.Core.Models.Base;

namespace Modeling.DeltaT.Algorithm.Models.Nodes;

[DebuggerDisplay("Queue [{Id}]")]
public sealed class Queue : QueueBase
{
	private readonly Dictionary<Guid, double> _queueEnterTimes = new();

	public Queue(QueueOptions? options = null) : base(options)
	{
	}

	public override void Process(Request request)
	{
		if (IsFull)
		{
			Context.Collector.CounterIncrement($"{Id}_Queue_Dropped");
			return;
		}

		_queueEnterTimes[request.Id] = Context.CurrentTime;
		Storage.Enqueue(request);
		Context.Collector.GaugeRecord($"{Id}_Queue_Size", Storage.Count);
	}

	public override void Update(double deltaTime)
	{
		Context.Collector.GaugeRecord($"{Id}_Queue_Size", Storage.Count);
		Context.Collector.ListAdd($"{Id}_Queue_Size_History", Storage.Count);

		if (IsEmpty)
			return;

		var next = GetAvailableExit();

		if (next == null)
			return;

		var req = Dequeue();

		if (req != null)
		{
			if (_queueEnterTimes.TryGetValue(req.Id, out var enterTime))
			{
				var waitTime = Context.CurrentTime - enterTime;
				Context.Collector.ListAdd($"{Id}_Queue_WaitTime", waitTime);
				_queueEnterTimes.Remove(req.Id);
			}

			next.Process(req);
		}
	}
}