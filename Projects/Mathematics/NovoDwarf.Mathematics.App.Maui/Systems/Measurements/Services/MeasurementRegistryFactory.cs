using NovoDwarf.Mathematics.App.Systems.Measurements.Core;

namespace NovoDwarf.Mathematics.App.Systems.Measurements.Services;

public static class MeasurementRegistryFactory
{
	public static InMemoryMeasurementRegistry Create(MeasurementSchema schema)
	{
		return new InMemoryMeasurementRegistry(
			quantities: schema.Quantities.Select(q => q.ToDomain()),
			systems: schema.Systems.Select(s => s.ToDomain()),
			baseUnits: schema.Units.Select(u => u.ToDomain()),
			prefixes: schema.Prefixes.Select(p => p.ToDomain())
		);
	}
}