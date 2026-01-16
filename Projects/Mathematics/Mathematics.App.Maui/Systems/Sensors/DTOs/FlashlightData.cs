using System.Numerics;
using Mathematics.App.Maui.Systems.Sensors.Enums;

namespace Mathematics.App.Maui.Systems.Sensors.DTOs;

public sealed record FlashlightData : IEqualityOperators<FlashlightData, FlashlightData, bool>
{
	public FlashlightModeType Type { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
}