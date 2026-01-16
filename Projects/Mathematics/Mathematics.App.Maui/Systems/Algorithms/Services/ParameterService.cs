using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LocalizationResourceManager.Maui;
using Mathematics.App.ViewModels.Common;
using Mathematics.Core.Attributes;
using Mathematics.Core.Resources;
using Microsoft.Extensions.Logging;

namespace Mathematics.App.Systems.Algorithms.Services;

public class ParameterService
{
	private readonly ILocalizationResourceManager _manager;
	private readonly ILogger<ParameterService> _logger;

	public ParameterService(ILogger<ParameterService> logger, ILocalizationResourceManager manager)
	{
		_logger = logger;
		_manager = manager;
	}

	public List<ParameterViewModel> GetParameters(Type entityType)
	{
		var parameters = new List<ParameterViewModel>();

		var props = entityType
			.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.Where(p => p.GetCustomAttribute<EntityParameterAttribute>() != null);

		var fields = entityType
			.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.Where(f => f.GetCustomAttribute<EntityParameterAttribute>() != null);

		foreach (var member in props.Cast<MemberInfo>().Concat(fields))
		{
			var attr = member.GetCustomAttribute<EntityParameterAttribute>()!;

			var vm = new ParameterViewModel
			{
				Name = Parameters_Resources.ResourceManager.GetString(attr.NameKey) ?? attr.NameKey,
				Description = Parameters_Resources.ResourceManager.GetString(attr.DescKey) ?? attr.DescKey,
				DataType = attr.Type,
				Value = GetDefaultValue(attr.Type)
			};

			var range = member.GetCustomAttribute<RangeAttribute>();

			if (range != null)
			{
				vm.Min = Convert.ToDouble(range.Minimum);
				vm.Max = Convert.ToDouble(range.Maximum);
			}

			parameters.Add(vm);
		}

		return parameters;
	}

	private static object GetDefaultValue(Type type)
	{
		if (type == typeof(int)) return 0;
		if (type == typeof(double)) return 0.0;
		if (type == typeof(bool)) return false;
		if (type == typeof(DateTime)) return DateTime.Now;
		return string.Empty;
	}
}