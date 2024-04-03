using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transmitly.Channel.Configuration;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	sealed class ServiceProviderChannelProviderFactory(
		IEnumerable<IChannelProviderRegistration> channelProviders,
		IEnumerable<IChannelProviderDeliveryReportRequestAdaptorRegistration> channelProviderDeliveryReportRequestAdaptorRegistrations,
		IServiceProvider serviceProvider) : BaseChannelProviderFactory(channelProviders, channelProviderDeliveryReportRequestAdaptorRegistrations)
	{
		public override Task<IChannelProviderClient> ResolveClientAsync(IChannelProviderRegistration channelProvider)
		{
			if (channelProvider.Configuration == null)
				return Task.FromResult((IChannelProviderClient)ActivatorUtilities.CreateInstance(serviceProvider, channelProvider.ClientType));
			else
				return Task.FromResult((IChannelProviderClient)ActivatorUtilities.CreateInstance(serviceProvider, channelProvider.ClientType, channelProvider.Configuration));
		}

		public override Task<IChannelProviderDeliveryReportRequestAdaptor> ResolveDeliveryReportRequestAdaptorAsync(IChannelProviderDeliveryReportRequestAdaptorRegistration channelProviderDeliveryReportRequestAdaptor)
		{
			return Task.FromResult((IChannelProviderDeliveryReportRequestAdaptor)ActivatorUtilities.CreateInstance(serviceProvider, channelProviderDeliveryReportRequestAdaptor.Type));
		}
	}
}