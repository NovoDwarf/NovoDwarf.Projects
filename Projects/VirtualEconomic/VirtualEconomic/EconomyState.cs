using VirtualEconomic.Systems.Populations;

namespace VirtualEconomic;

public sealed class EconomyState
{
	public EntityStore World;
	public PopulationSystem Population;
	public Random Random;
	
	public EconomyState(EntityStore world)
	{
		World = world;
	}
}

public sealed class EconomyStatistics
{
	public long Population { get; set; }

	public decimal GDP { get; set; }

	public decimal MoneySupply { get; set; }

	public double Unemployment { get; set; }

	public double Inflation { get; set; }
}