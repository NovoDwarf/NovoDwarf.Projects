using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Messager.Core;
using Messager.Entity.Receivers;
using Messager.Entity.Senders;
using Messager.Interfaces.Factories;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Microsoft.Extensions.Logging;

namespace Messager;

public sealed class MessagerModule : Module
{
	private readonly MessagerOptions? _options;

	public MessagerModule(MessagerOptions? options = null)
	{
		_options = options ?? new MessagerOptions();
	}

	protected override void Load(ContainerBuilder builder)
	{
		builder.Register(c =>
			{
				var factory = _options is { EnableLogging: true } 
					? c.Resolve<ILoggerFactory>() 
					: null;
				
				return new Exchange(factory);
			})
			.As<ISimpleBrokerFactory>()
			.As<IKeyedMessageBrokerFactory>()
			.SingleInstance();

		builder.RegisterGeneric(typeof(Sender<>))
			.As(typeof(ISender<>))
			.InstancePerDependency();

		builder.RegisterGeneric(typeof(Receiver<>))
			.As(typeof(IReceiver<>))
			.InstancePerDependency();

		builder.RegisterGeneric(typeof(KeyedSender<,>))
			.As(typeof(ISender<,>))
			.InstancePerDependency();

		builder.RegisterGeneric(typeof(KeyedReceiver<,>))
			.As(typeof(IReceiver<,>))
			.InstancePerDependency();
		
		builder.RegisterSource(new AutoSenderRegistrationSource());
		builder.RegisterSource(new AutoReceiverRegistrationSource());
	}

	private class AutoSenderRegistrationSource : IRegistrationSource
	{
		public bool IsAdapterForIndividualComponents => false;

		public IEnumerable<IComponentRegistration> RegistrationsFor(
			Service service, 
			Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
		{
			if (service is not IServiceWithType swt || !swt.ServiceType.IsGenericType)
				yield break;
			
			var genericType = swt.ServiceType.GetGenericTypeDefinition();
			
			if (genericType != typeof(ISender<,>))
				yield break;
			
			var args = swt.ServiceType.GetGenericArguments();
			var concreteType = typeof(KeyedSender<,>).MakeGenericType(args);
			var registration = RegistrationBuilder
				.ForType(concreteType)
				.As(service)
				.InstancePerDependency()
				.CreateRegistration();
			
			yield return registration;
		}
	}

	private class AutoReceiverRegistrationSource : IRegistrationSource
	{
		public bool IsAdapterForIndividualComponents => false;

		public IEnumerable<IComponentRegistration> RegistrationsFor(
			Service service, 
			Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
		{
			if (service is not IServiceWithType swt || !swt.ServiceType.IsGenericType)
				yield break;
			
			var genericType = swt.ServiceType.GetGenericTypeDefinition();
			
			if (genericType != typeof(IReceiver<,>))
				yield break;
			
			var args = swt.ServiceType.GetGenericArguments();
			var concreteType = typeof(KeyedReceiver<,>).MakeGenericType(args);
			var registration = RegistrationBuilder
				.ForType(concreteType)
				.As(service)
				.InstancePerDependency()
				.CreateRegistration();
			
			yield return registration;
		}
	}
}


