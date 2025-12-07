using System.ComponentModel;
using System.Reflection;
using Mathematics.App.Maui.Base;
using Mathematics.Core.Attributes;

namespace Mathematics.App.Maui.Services;

public partial class ParameterViewModel : BaseViewModel
{
	public string Name { get; set; }
	public string Description { get; set; }
	public Type DataType { get; set; }
    
	public object Value { get; set; }
}

public class ParameterService
{
	public static List<ParameterViewModel> GetParameters(Type entityType)
	{
		var parameters = new List<ParameterViewModel>();
        
		var attribute = entityType.GetCustomAttribute<EntityParameterAttribute>();
		if (attribute == null) return parameters;
        
		/*for (int i = 0; i < attribute.ParameterKeys.Length; i++)
		{
			var param = new ParameterViewModel
			{
				Name = attribute.ParameterKeys[i],
				Description = i < attribute.Descriptions.Length ? attribute.Descriptions[i] : string.Empty,
				DataType = i < attribute.ParameterTypes.Length ? attribute.ParameterTypes[i] : typeof(string)
			};
            
			// Установка значения по умолчанию в зависимости от типа
			param.Value = GetDefaultValue(param.DataType);
            
			parameters.Add(param);
		}*/
        
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