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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transmitly.Model.Configuration;
using Transmitly.Util;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	sealed class ServiceProviderContentModelEnricherRegistrationFactory(IEnumerable<IContentModelEnricherRegistration> registrations, IServiceProvider serviceProvider) : IContentModelEnricherFactory
	{
		private readonly IReadOnlyList<IContentModelEnricherRegistration> _registrations = [.. Guard.AgainstNull(registrations)];

		public Task<IReadOnlyList<IContentModelEnricherRegistration>> GetAllEnrichersAsync()
		{
			return Task.FromResult(_registrations);
		}

		public Task<IContentModelEnricher?> GetEnricher(IContentModelEnricherRegistration registration)
		{
			Guard.AgainstNull(registration);
			return Task.FromResult((IContentModelEnricher?)serviceProvider.GetService(registration.EnricherType));
		}
	}
}
