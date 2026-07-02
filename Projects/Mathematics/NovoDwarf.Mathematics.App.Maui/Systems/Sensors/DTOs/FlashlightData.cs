using System.Numerics;
using NovoDwarf.Mathematics.App.Systems.Sensors.Enums;

namespace NovoDwarf.Mathematics.App.Systems.Sensors.DTOs;

public sealed record FlashlightData : IEqualityOperators<FlashlightData, FlashlightData, bool>
{
	public FlashlightModeType Type { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
}