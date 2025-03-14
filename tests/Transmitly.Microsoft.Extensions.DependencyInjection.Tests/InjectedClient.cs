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
			public void DeliverReport(DeliveryReport report)
			{
				Client.DeliverReport(report);
			}

			public void DeliverReports(IReadOnlyCollection<DeliveryReport> reports)
			{
				Client.DeliverReports(reports);
			}

			public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentityProfile> platformIdentities, ITransactionModel transactionalModel, IReadOnlyCollection<string> channelPreferences, string? cultureInfo = null, CancellationToken cancellationToken = default)
			{
				return Client.DispatchAsync(pipelineName, platformIdentities, transactionalModel, channelPreferences, cultureInfo, cancellationToken);
			}

			public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentityReference> identityReferences, ITransactionModel transactionalModel, IReadOnlyCollection<string> channelPreferences, string? cultureInfo = null, CancellationToken cancellationToken = default)
			{
				return Client.DispatchAsync(pipelineName, identityReferences, transactionalModel, channelPreferences, cultureInfo, cancellationToken);
			}
		}

	}
}