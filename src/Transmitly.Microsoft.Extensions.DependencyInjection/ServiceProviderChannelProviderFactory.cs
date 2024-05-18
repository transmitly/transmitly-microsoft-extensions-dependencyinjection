using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transmitly.Delivery;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Verification;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
    sealed class ServiceProviderChannelProviderFactory(
        IEnumerable<IChannelProviderRegistration> channelProviders,
        IServiceProvider serviceProvider) : BaseChannelProviderFactory(channelProviders)
    {
        public override Task<IChannelProviderClient?> ResolveClientAsync(IChannelProviderRegistration channelProvider, IChannelProviderClientRegistration channelProviderClientRegistration)
        {
            if (channelProvider.Configuration == null)
                return Task.FromResult((IChannelProviderClient?)ActivatorUtilities.CreateInstance(serviceProvider, channelProviderClientRegistration.ClientType));
            else
                return Task.FromResult((IChannelProviderClient?)ActivatorUtilities.CreateInstance(serviceProvider, channelProviderClientRegistration.ClientType, channelProvider.Configuration));
        }

        public override Task<IChannelProviderDeliveryReportRequestAdaptor> ResolveDeliveryReportRequestAdaptorAsync(IDeliveryReportRequestAdaptorRegistration channelProviderDeliveryReportRequestAdaptor)
        {
            return Task.FromResult((IChannelProviderDeliveryReportRequestAdaptor)ActivatorUtilities.CreateInstance(serviceProvider, channelProviderDeliveryReportRequestAdaptor.Type));
        }

        public override Task<IChannelVerificationChannelProviderClient> ResolveChannelVerificationClientAsync(IChannelVerificationClientRegistration channelVerificationClientRegistration)
        {
            if (channelVerificationClientRegistration.Configuration == null)
                return Task.FromResult((IChannelVerificationChannelProviderClient)ActivatorUtilities.CreateInstance(serviceProvider, channelVerificationClientRegistration.ClientType));
            else
                return Task.FromResult((IChannelVerificationChannelProviderClient)ActivatorUtilities.CreateInstance(serviceProvider, channelVerificationClientRegistration.ClientType, channelVerificationClientRegistration.Configuration));
        }
    }
}