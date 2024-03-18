using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transmitly.Channel.Configuration;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	sealed class ServiceProviderChannelProviderFactory(IEnumerable<IChannelProviderRegistration> channelProviders, IServiceProvider serviceProvider) : BaseChannelProviderFactory(channelProviders)
	{
		public override Task<IChannelProviderClient> ResolveClientAsync(IChannelProviderRegistration channelProvider)
		{
			return Task.FromResult((IChannelProviderClient)ActivatorUtilities.CreateInstance(serviceProvider, channelProvider.ClientType, channelProvider.Configuration));
		}
	}
}