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
using Transmitly.PlatformIdentity.Configuration;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
	public partial class TransmitlyDependencyInjectionExtensionsTests
	{
		sealed class ProfileEnricherTracker
		{
			private readonly List<string> _executionOrder = [];
			private readonly object _lock = new();
			public int InvocationCount { get; private set; }

			public IReadOnlyList<string> ExecutionOrder
			{
				get
				{
					lock (_lock)
					{
						return _executionOrder.ToList().AsReadOnly();
					}
				}
			}

			public void Track(string marker)
			{
				lock (_lock)
				{
					InvocationCount++;
					_executionOrder.Add(marker);
				}
			}
		}

		abstract class TrackingProfileEnricher(ProfileEnricherTracker tracker, string marker) : IPlatformIdentityProfileEnricher
		{
			private readonly ProfileEnricherTracker _tracker = tracker;
			private readonly string _marker = marker;

			public virtual Task EnrichIdentityProfileAsync(IPlatformIdentityProfile identityProfile)
			{
				_tracker.Track(_marker);
				return Task.CompletedTask;
			}
		}

		sealed class CountingProfileEnricher(ProfileEnricherTracker tracker) : TrackingProfileEnricher(tracker, "count");
		sealed class OrderedFirstProfileEnricher(ProfileEnricherTracker tracker) : TrackingProfileEnricher(tracker, "first");
		sealed class OrderedSecondProfileEnricher(ProfileEnricherTracker tracker) : TrackingProfileEnricher(tracker, "second");
		sealed class OrderedThirdProfileEnricher(ProfileEnricherTracker tracker) : TrackingProfileEnricher(tracker, "third");
		sealed class TypeScopedGlobalProfileEnricher(ProfileEnricherTracker tracker) : TrackingProfileEnricher(tracker, "global");
		sealed class TypeScopedMemberProfileEnricher(ProfileEnricherTracker tracker) : TrackingProfileEnricher(tracker, "member");
		sealed class TypeScopedOtherProfileEnricher(ProfileEnricherTracker tracker) : TrackingProfileEnricher(tracker, "other");

		sealed class ThrowingProfileEnricher : IPlatformIdentityProfileEnricher
		{
			public Task EnrichIdentityProfileAsync(IPlatformIdentityProfile identityProfile)
			{
				throw new InvalidOperationException("Intentional test exception");
			}
		}
	}
}
