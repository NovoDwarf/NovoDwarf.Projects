using NovoDwarf.Mathematics.App.Systems.Measurements.Domain;

namespace NovoDwarf.Mathematics.App.Systems.Measurements.Interfaces;

public interface IMeasurementRegistry
{
	public IReadOnlyCollection<Quantity> Quantities { get; }
	public IReadOnlyCollection<UnitSystem> Systems { get; }
	public IReadOnlyCollection<Unit> Units { get; }
	public IReadOnlyCollection<Prefix> Prefixes { get; }

	public Unit GetUnit(string unitId);
	public Quantity GetQuantity(string quantityId);
}