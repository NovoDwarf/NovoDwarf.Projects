using System.Diagnostics;
using Structures.Models.Abstracts.Nodes;
using Structures.Models.Abstracts.Options;
using Structures.Models.Base;

namespace DeltaT.Algorithm.Models.Nodes;

[DebuggerDisplay("Sink [{Id}]")]
public class Sink : SinkBase
{
	public Sink(SinkOptions? options = null) : base(options) { }
	
	public override void Process(Request request)
	{
		Context.Collector.CounterIncrement($"{Id}_Sink_Completed");
		//sim.CompleteRequest(request);
	}
}