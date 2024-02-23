using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transmitly.Channel.Configuration;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	class ServiceProviderChannelProviderFactory : BaseChannelProviderFactory
	{
		private readonly IServiceProvider _serviceProvider;

		public ServiceProviderChannelProviderFactory(IEnumerable<IChannelProviderRegistration> channelProviders, IServiceProvider serviceProvider) : base(channelProviders)
		{
			_serviceProvider = serviceProvider;
		}

		public override Task<IChannelProviderClient> ResolveClientAsync(IChannelProviderRegistration channelProvider)
		{
			return Task.FromResult((IChannelProviderClient)_serviceProvider.GetRequiredService(channelProvider.ClientType));
		}
	}
}