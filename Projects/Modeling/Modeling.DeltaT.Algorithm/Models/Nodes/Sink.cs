using System.Diagnostics;
using Modeling.Core.Models.Abstracts.Nodes;
using Modeling.Core.Models.Abstracts.Options;
using Modeling.Core.Models.Base;

namespace Modeling.DeltaT.Algorithm.Models.Nodes;

[DebuggerDisplay("Sink [{Id}]")]
public class Sink : SinkBase
{
	public Sink(SinkOptions? options = null) : base(options)
	{
	}

	public override void Process(Request request)
	{
		Context.Collector.CounterIncrement($"{Id}_Sink_Completed");
		//sim.CompleteRequest(request);
	}
}