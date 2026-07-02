using NovoDwarf.Mathematics.App.Systems.Measurements.Core;
using NovoDwarf.Mathematics.App.Systems.Measurements.DTOs;
using NovoDwarf.Mathematics.App.Systems.Utilities;
using Serilog;

namespace NovoDwarf.Mathematics.App.Systems.Measurements.Services;

public static class MeasurementLoader
{
	public static async Task<InMemoryMeasurementRegistry> LoadFromRawAsync()
	{
		var quantitiesJson = await ReadRawFileAsync("Measurements/Quantities.json");
		var unitsJson = await ReadRawFileAsync("Measurements/Units.json");
		var systemsJson = await ReadRawFileAsync("Measurements/Systems.json");
		var prefixesJson = await ReadRawFileAsync("Measurements/Prefixes.json");

		var quantities = JsonUtils.Deserialize<List<QuantityDto>>(quantitiesJson) ?? [];
		var units = JsonUtils.Deserialize<List<UnitDto>>(unitsJson) ?? [];
		var systems = JsonUtils.Deserialize<List<UnitSystemDto>>(systemsJson) ?? [];
		var prefixes = JsonUtils.Deserialize<List<PrefixDto>>(prefixesJson) ?? [];

		var schema = new MeasurementSchema
		{
			Quantities = quantities,
			Units = units,
			Systems = systems,
			Prefixes = prefixes
		};

		return MeasurementRegistryFactory.Create(schema);
	}

	private static async Task<string> ReadRawFileAsync(string fileName)
	{
		try
		{
			await using var stream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
			using var reader = new StreamReader(stream);
			var st = reader.ReadToEnd();

			return st;
		}
		catch (Exception exception)
		{
			Log.Error(exception, "Error in reading file");
		}

		return string.Empty;
	}
}