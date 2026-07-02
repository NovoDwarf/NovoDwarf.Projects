using Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;
using Modeling.Core.Extensions;
using Modeling.Core.Models.Abstracts.Options;
using NovoDwarf.Modeling.EventDriven.Models.Nodes;
using NovoDwarf.Modeling.EventDriven.Models.Simulations;

namespace NovoDwarf.Modeling.EventDriven.Example;

internal static class Program
{
	public static void Main(string[] args)
	{
		StartConsoleSim();
	}

	private static void StartConsoleSim()
	{
		var sim = new EventDrivenSimulation();
		
		var sourceExpo = new ExpoDistribution();
		var serviceExpo = new ExpoDistribution();

		sourceExpo.Set(10);
		serviceExpo.Set(200);
		
		var collector = sim.EventCollector;

		var g1 = new Source(new SourceOptions { Distribution = sourceExpo, });
		var g2 = new Source(new SourceOptions { Distribution = sourceExpo, });
		var g3 = new Source(new SourceOptions { Distribution = sourceExpo, });
		var g4 = new Source(new SourceOptions { Distribution = sourceExpo, });

		var q1 = new Queue();
		var q2 = new Queue();

		var u1 = new Service(collector, new ServiceOptions { Distribution = serviceExpo });
		var u2 = new Service(collector, new ServiceOptions { Distribution = serviceExpo });

		var s1 = new Sink();

		g1.Connect(q1);
		g2.Connect(q1);
		g3.Connect(q1);
		g4.Connect(q1);

		q1.Connect(u1).Connect(q2).Connect(u2).Connect(s1);

		sim.AddNodes(g1, g2, g3, g4, q1, q2, u1, u2, s1);
		sim.Simulate();
	}
}