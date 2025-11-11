using Autofac;
using Microsoft.Extensions.Logging;

namespace Messager.Extensions;

public static class ContainerBuilderExtensions
{
	public static ContainerBuilder AddEventSystem(this ContainerBuilder builder, Action<MessagerOptions>? configureOptions = null)
	{
		var options = new MessagerOptions();
		
		configureOptions?.Invoke(options);
		
		builder.RegisterModule(new MessagerModule(options));
		
		return builder;
	}
}