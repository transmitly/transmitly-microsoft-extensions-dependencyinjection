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

using Transmitly;
using Transmitly.Delivery;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
	public partial class TransmitlyDependencyInjectionExtensionsTests
	{
		class InjectedClient : ICommunicationsClient
		{
			private readonly Lazy<ICommunicationsClient> _lazy;

			public InjectedClient(ICommunicationsClientConfigurator configurator)
			{
				_lazy = new Lazy<ICommunicationsClient>(() =>
				{
					var cfg = new CommunicationsClientBuilder();
					configurator.ConfigureClient(cfg);
					return cfg.BuildClient();
				});
			}

			public ICommunicationsClient Client
			{
				get
				{
					return _lazy.Value;
				}
			}

			public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineIntent, IReadOnlyCollection<IPlatformIdentityProfile> platformIdentities, ITransactionModel transactionalModel, IReadOnlyCollection<string> dispatchChannelPreferences, string? pipelineId = null, string? cultureInfo = null, CancellationToken cancellationToken = default)
			{
				return Client.DispatchAsync(pipelineIntent, platformIdentities, transactionalModel, dispatchChannelPreferences, pipelineId, cultureInfo, cancellationToken);
			}

			public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineIntent, IReadOnlyCollection<IPlatformIdentityReference> identityReferences, ITransactionModel transactionalModel, IReadOnlyCollection<string> dispatchChannelPreferences, string? pipelineId = null, string? cultureInfo = null, CancellationToken cancellationToken = default)
			{
				return Client.DispatchAsync(pipelineIntent, identityReferences, transactionalModel, dispatchChannelPreferences, pipelineId, cultureInfo, cancellationToken);
			}

			public Task DispatchAsync(DeliveryReport report)
			{
				return Client.DispatchAsync(report);
			}

			public Task DispatchAsync(IReadOnlyCollection<DeliveryReport> reports)
			{
				return Client.DispatchAsync(reports);
			}
		}

	}
}