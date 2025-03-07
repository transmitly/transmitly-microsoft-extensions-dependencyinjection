﻿// ﻿﻿Copyright (c) Code Impressions, LLC. All Rights Reserved.
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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Transmitly.Delivery;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	sealed class EmptyClient : ICommunicationsClient
	{
		public void DeliverReport(DeliveryReport report)
		{
			throw new NotImplementedException();
		}

		public void DeliverReports(IReadOnlyCollection<DeliveryReport> reports)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentity> platformIdentities, ITransactionModel transactionalModel, IReadOnlyCollection<string> allowedChannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityAddress> identityAddresses, ITransactionModel transactionalModel, IReadOnlyCollection<string> allowedChannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, string identityAddress, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, string identityAddress, object transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<string> identityAddresses, object transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityAddress> identityAddresses, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentity> platformIdentities, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityReference> identityReferences, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityReference> identityReferences, ITransactionModel transactionalModel, IReadOnlyCollection<string> allowedCHannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}