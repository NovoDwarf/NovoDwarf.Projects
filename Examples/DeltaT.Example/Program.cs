using DeltaT.Algorithm.Models.Nodes;
using DeltaT.Algorithm.Models.Simulations;
using Mathematics.Models.Distributions.Basic;
using Structures.Extensions;
using Structures.Models.Abstracts.Options;

namespace DeltaT.Example;

public static class Program
{
	public static void Main(string[] args)
	{
		StartConsoleSim();
	}

	private static void StartConsoleSim()
	{
		var g1 = new Source(new SourceOptions { Distribution = new Exponential(5) });
		var g2 = new Source(new SourceOptions { Distribution = new Exponential(5) });
		var g3 = new Source(new SourceOptions { Distribution = new Exponential(5) });
		var g4 = new Source(new SourceOptions { Distribution = new Exponential(5) });

		var q1 = new Queue();
		var q2 = new Queue();

		var u1 = new Service(new ServiceOptions { Distribution = new Exponential(5) });
		var u2 = new Service(new ServiceOptions { Distribution = new Exponential(5) });

		var s1 = new Sink();
		
		g1.Connect(q1);
		g2.Connect(q1);
		g3.Connect(q1);
		g4.Connect(q1);
		
		q1.Connect(u1).Connect(q2).Connect(u2).Connect(s1);
		
		var sim = new SequentialSimulation();
		
		sim.AddNodes(g1, g2, g3, g4, q1, q2, u1, u2, s1);
		sim.Simulate();
	}
}