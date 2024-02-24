﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Pipeline.Configuration;
using Transmitly.Template.Configuration;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	class ServiceCollectionCommunicationClientFactory : BaseCommunicationClientFactory
	{
		private readonly IServiceCollection _services;

		public ServiceCollectionCommunicationClientFactory(IServiceCollection services)
		{
			_services = services ?? throw new ArgumentNullException(nameof(services));
		}

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
					throw new NotImplementedException();
			}
			foreach (var templateEngine in context.TemplateEngines)
			{
				_services.AddSingleton(templateEngine);
			}
			_services.AddSingleton(context.DeliveryReportProvider);
			_services.AddSingleton(context.CommunicationsConfigurationSettings);
			_services.AddSingleton<ITemplateEngineRegistrationStore, InMemoryTemplateEngineRegistrationStore>();
			_services.AddSingleton<IPipelineFactory, DefaultPipelineFactory>();
			_services.AddSingleton<IChannelProviderFactory, ServiceProviderChannelProviderFactory>();
			_services.AddSingleton<ICommunicationsClient, DefaultCommunicationsClient>();
			return null;
		}
	}
}