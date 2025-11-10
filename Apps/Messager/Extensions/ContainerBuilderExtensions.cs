using Autofac;

namespace Messager.Extensions;

/// <summary>
/// Расширения для ContainerBuilder для удобной регистрации Messager модуля
/// </summary>
public static class ContainerBuilderExtensions
{
	/// <summary>
	/// Регистрирует Messager систему событий как Autofac Module
	/// </summary>
	/// <param name="builder">ContainerBuilder для регистрации</param>
	/// <returns>ContainerBuilder для цепочки вызовов</returns>
	public static ContainerBuilder AddEventSystem(this ContainerBuilder builder)
	{
		builder.RegisterModule<MessagerModule>();
		return builder;
	}
}