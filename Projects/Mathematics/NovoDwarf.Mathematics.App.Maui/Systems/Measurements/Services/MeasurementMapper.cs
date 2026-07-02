using NovoDwarf.Mathematics.App.Systems.Measurements.Domain;
using NovoDwarf.Mathematics.App.Systems.Measurements.DTOs;

namespace NovoDwarf.Mathematics.App.Systems.Measurements.Services;

public static class MeasurementMapper
{
	public static Quantity ToDomain(this QuantityDto dto)
	{
		return new Quantity
		{
			Id = dto.Id,
			DisplayName = dto.DisplayName,
			Dimension = new Dimension(dto.Dimension),
			CanonicalUnitId = dto.CanonicalUnitId
		};
	}

	public static UnitSystem ToDomain(this UnitSystemDto dto)
	{
		return new UnitSystem
		{
			Id = dto.Id,
			DisplayName = dto.DisplayName
		};
	}

	public static Unit ToDomain(this UnitDto dto)
	{
		return new Unit
		{
			Id = dto.Id,
			DisplayName = dto.DisplayName,
			QuantityId = dto.QuantityId,
			SystemId = dto.SystemId,
			Factor = dto.Factor,
			Offset = dto.Offset,
			AllowPrefixes = dto.AllowPrefixes
		};
	}

	public static Prefix ToDomain(this PrefixDto dto)
	{
		return new Prefix
		{
			Id = dto.Id,
			DisplayName = dto.DisplayName,
			Power = dto.Power
		};
	}
}