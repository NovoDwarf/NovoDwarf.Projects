
using Mathematics.Distributions.Univariate.Continuous.Semibounded;
using Modeling.Core.Extensions;
using Modeling.Core.Models.Abstracts.Options;
using Modeling.DeltaT.Algorithm.Models.Nodes;
using Modeling.DeltaT.Algorithm.Models.Simulations;


namespace Modeling.DeltaT.Example;

internal static class Program
{
	public static void Main(string[] args)
	{
		StartConsoleSim();
	}

	private static void StartConsoleSim()
	{
		var sim = new SequentialSimulation();
		
		var sourceExpo = new ExpoDistribution();
		var serviceExpo = new ExpoDistribution();
		
		sourceExpo.Set(10);
		serviceExpo.Set(200);
		
		var g1 = new Source(new SourceOptions { Distribution = sourceExpo, ClosedSystem = true, ClosedPopulation = 1 });
		var g2 = new Source(new SourceOptions { Distribution = sourceExpo, ClosedSystem = true, ClosedPopulation = 1 });
		var g3 = new Source(new SourceOptions { Distribution = sourceExpo, ClosedSystem = true, ClosedPopulation = 1 });
		var g4 = new Source(new SourceOptions { Distribution = sourceExpo, ClosedSystem = true, ClosedPopulation = 1 });

		var q1 = new Queue();
		var q2 = new Queue();

		var u1 = new Service(new ServiceOptions { Distribution = serviceExpo });
		var u2 = new Service(new ServiceOptions { Distribution = serviceExpo });

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