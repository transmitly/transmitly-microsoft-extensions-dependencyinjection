using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Pipeline.Configuration;
using Transmitly.Template.Configuration;
using Transmitly.Verification.Configuration;

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

                foreach (var clientType in channelProvider.ClientRegistrations.Select(s => s.ClientType))
                {
                    if (channelProvider.Configuration == null)
                    {
                        _services.AddSingleton(clientType);
                    }
                    else
                    {
                        _services.AddSingleton(clientType, provider => ActivatorUtilities.CreateInstance(provider, clientType, channelProvider.Configuration));
                    }


                }
                
                foreach (var verify in channelProvider.ChannelVerificationClientRegistrations)
                {
                    _services.AddSingleton(verify);
                    if (verify.Configuration == null)
                    {
                        _services.AddSingleton(verify.ClientType);
                    }
                    else
                    {
                        _services.AddSingleton(verify.ClientType, provider => ActivatorUtilities.CreateInstance(provider, verify.ClientType, verify.Configuration));
                    }
                }

                foreach (var channelProviderAdaptorRegistration in channelProvider.DeliveryReportRequestAdaptorRegistrations)
                {
                    _services.AddSingleton(channelProviderAdaptorRegistration);
                    _services.AddSingleton(channelProviderAdaptorRegistration.Type);
                }
            }

            foreach (var templateEngine in context.TemplateEngines)
            {
                _services.AddSingleton(templateEngine);
            }


            _services.AddSingleton(context.DeliveryReportProvider);
            _services.AddSingleton<ITemplateEngineFactory, DefaultTemplateEngineFactory>();
            _services.AddSingleton<IPipelineFactory, DefaultPipelineFactory>();
            _services.AddSingleton<IChannelProviderFactory, ServiceProviderChannelProviderFactory>();
            _services.AddSingleton<ICommunicationsClient, DefaultCommunicationsClient>();
            _services.AddSingleton(context.ChannelVerificationConfiguration);
            _services.AddSingleton<IChannelVerificationCommunicationsClient, DefaultChannelVerificationCommunicationsClient>();
            return new EmptyClient();
        }
    }
}