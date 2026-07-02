using NovoDwarf.Mathematics.App.Systems.Measurements.DTOs;

namespace NovoDwarf.Mathematics.App.Systems.Measurements.Core;

public sealed class MeasurementSchema
{
	public List<QuantityDto> Quantities { get; init; } = [];
	
	public List<UnitSystemDto> Systems { get; init; } = [];
	
	public List<UnitDto> Units { get; init; } = [];
	
	public List<PrefixDto> Prefixes { get; init; } = [];
}