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
using Transmitly.Microsoft.Extensions.DependencyInjection.Tests;
using Transmitly.PlatformIdentity.Configuration;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
	public partial class TransmitlyDependencyInjectionExtensionsTests
	{
		class IdentityResolverWithDependency(SimpleDependency dep) : IPlatformIdentityResolver
		{
			public static SimpleDependency? CapturedDependency { get; set; }
			private readonly SimpleDependency _dep = dep;

			public Task<IReadOnlyCollection<IPlatformIdentityProfile>?> ResolveIdentityProfiles(
				IReadOnlyCollection<IPlatformIdentityReference> identityReferences)
			{
				CapturedDependency = _dep;
				return Task.FromResult<IReadOnlyCollection<IPlatformIdentityProfile>?>(Array.Empty<IPlatformIdentityProfile>());
			}
		}
	}
}
