using Mathematics.App.Maui.Systems.Measurements.DTOs;

namespace Mathematics.App.Maui.Systems.Measurements.Core;

public sealed class MeasurementSchema
{
	public List<QuantityDto> Quantities { get; init; } = [];
	
	public List<UnitSystemDto> Systems { get; init; } = [];
	
	public List<UnitDto> Units { get; init; } = [];
	
	public List<PrefixDto> Prefixes { get; init; } = [];
}