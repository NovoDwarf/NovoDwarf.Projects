using System.Text.Json;
using Serilog;

namespace NovoDwarf.Mathematics.App.Systems.Utilities;

public static class JsonUtils
{
	private static readonly JsonSerializerOptions Settings = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true,
	};

	public static T? Deserialize<T>(string json)
	{
		if (string.IsNullOrEmpty(json))
			return default;

		try
		{
			return JsonSerializer.Deserialize<T>(json, Settings);
		}
		catch (JsonException exception)
		{
			Log.Error(exception, "JSON is not a valid JSON");
		}
		catch (NotSupportedException exception)
		{
			Log.Error(exception, "No converters found");
		}
		catch (Exception exception)
		{
			Log.Error(exception, "Error in deserialization JSON");
		}

		return default;
	}

	public static string Serialize<T>(T t)
	{
		try
		{
			return JsonSerializer.Serialize(t, Settings);
		}
		catch (NotSupportedException exception)
		{
			Log.Error(exception, "No converters found");
		}
		catch (Exception exception)
		{
			Log.Error(exception, "Error in serialization JSON");
		}

		return string.Empty;
	}
}