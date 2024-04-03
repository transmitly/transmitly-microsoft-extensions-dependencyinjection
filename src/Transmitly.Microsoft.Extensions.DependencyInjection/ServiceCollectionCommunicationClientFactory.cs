using Microsoft.Extensions.DependencyInjection;
using System;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Pipeline.Configuration;
using Transmitly.Template.Configuration;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	sealed class ServiceCollectionCommunicationClientFactory(IServiceCollection services) : BaseCommunicationClientFactory
	{
		private readonly IServiceCollection _services = Guard.AgainstNull(services);

		public override ICommunicationsClient CreateClient(ICreateCommunicationsClientContext context)
		{
			foreach (var pipeline in context.Pipelines)
			{
				_services.AddSingleton(pipeline);
			}

			foreach (var channelProvider in context.ChannelProviders)
			{
				_services.AddSingleton(channelProvider);
				if (channelProvider.Configuration == null)
				{
					_services.AddSingleton(channelProvider.ClientType);
				}
				else
				{
					_services.AddSingleton(channelProvider.ClientType, provider => ActivatorUtilities.CreateInstance(provider, channelProvider.ClientType, channelProvider.Configuration));
				}
			}

			foreach (var channelProviderAdaptorRegistration in context.ChannelProviderDeliveryReportRequestAdaptors)
			{
				_services.AddSingleton(channelProviderAdaptorRegistration);
				_services.AddSingleton(channelProviderAdaptorRegistration.Type);
			}

			foreach (var templateEngine in context.TemplateEngines)
			{
				_services.AddSingleton(templateEngine);
			}
			_services.AddSingleton(context.DeliveryReportProvider);
			_services.AddSingleton(context.CommunicationsConfigurationSettings);
			_services.AddSingleton<ITemplateEngineFactory, DefaultTemplateEngineFactory>();
			_services.AddSingleton<IPipelineFactory, DefaultPipelineFactory>();
			_services.AddSingleton<IChannelProviderFactory, ServiceProviderChannelProviderFactory>();
			_services.AddSingleton<ICommunicationsClient, DefaultCommunicationsClient>();
			//_services.AddHttpClient();
			return new EmptyClient();
		}
	}
}