// ﻿﻿Copyright (c) Code Impressions, LLC. All Rights Reserved.
//  
//  Licensed under the Apache License, Version 2.0 (the "License")
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Persona.Configuration;
using Transmitly.Pipeline.Configuration;
using Transmitly.PlatformIdentity.Configuration;
using Transmitly.Template.Configuration;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
    sealed class ServiceCollectionCommunicationClientFactory(IServiceCollection services) : BaseCommunicationClientFactory
    {
        private readonly IServiceCollection _services = Guard.AgainstNull(services);

        public override ICommunicationsClient CreateClient(ICreateCommunicationsClientContext context)
        {
            foreach (var pipelineRegistration in context.Pipelines)
            {
                _services.AddSingleton(pipelineRegistration);
            }

            foreach (var channelProvider in context.ChannelProviders)
            {
                _services.AddSingleton(channelProvider);

                foreach (var dispatcherType in channelProvider.DispatcherRegistrations.Select(s => s.DispatcherType))
                {
                    if (channelProvider.Configuration == null)
                    {
                        _services.AddSingleton(dispatcherType);
                    }
                    else
                    {
                        _services.AddSingleton(dispatcherType, provider => ActivatorUtilities.CreateInstance(provider, dispatcherType, channelProvider.Configuration));
                    }
                }

                foreach (var channelProviderAdaptorRegistration in channelProvider.DeliveryReportRequestAdaptorRegistrations)
                {
                    _services.AddSingleton(channelProviderAdaptorRegistration);
                    _services.AddSingleton(channelProviderAdaptorRegistration.Type);
                }
            }

            foreach (var templateEngineRegistration in context.TemplateEngines)
            {
                _services.AddSingleton(templateEngineRegistration);
            }

            foreach (var identityResolverRegistration in context.PlatformIdentityResolvers)
            {
                _services.AddSingleton(identityResolverRegistration);
            }

            foreach (var personaRegistration in context.Personas)
            {
                _services.AddSingleton(personaRegistration);
            }

            _services.AddSingleton(context.DeliveryReportProvider);
            _services.AddSingleton<ITemplateEngineFactory, DefaultTemplateEngineFactory>();
            _services.AddSingleton<IPipelineFactory, DefaultPipelineFactory>();
            _services.AddSingleton<IChannelProviderFactory, ServiceProviderChannelProviderFactory>();
            _services.AddSingleton<ICommunicationsClient, DefaultCommunicationsClient>();
            _services.AddSingleton<IPersonaFactory, DefaultPersonaFactory>();
            _services.AddSingleton<IPlatformIdentityResolverFactory, ServiceProviderPlatformIdentityResolverRegistrationFactory>();

            return new EmptyClient();
        }
    }
}