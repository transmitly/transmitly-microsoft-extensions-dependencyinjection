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

using Moq;
using Transmitly;
using Transmitly.Microsoft.Extensions.DependencyInjection.Tests;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
	[TestClass()]
	public partial class TransmitlyDependencyInjectionExtensionsTests
	{
		[TestMethod]
		public void AddTransmitlyShouldCallConfigureClientOnResolution()
		{
			var services = new ServiceCollection();
			var tracker = new ConfigureClientTracker();

			services.AddSingleton(tracker);
			services.AddTransmitly(typeof(MockClientConfigurator));

			var provider = services.BuildServiceProvider();

			var client = provider.GetRequiredService<ICommunicationsClient>();
			client.DispatchAsync([]);

			Assert.IsTrue(tracker.WasCalled, "ConfigureClient was not called on MockClientConfigurator.");
		}

		[TestMethod]
		public void AddTransmitlyShouldCallConfigureClientOnResolution2()
		{
			// Arrange
			var services = new ServiceCollection();
			var configuratorMock = new Mock<ICommunicationsClientConfigurator>();
			configuratorMock
			.Setup(x => x.ConfigureClient(It.IsAny<CommunicationsClientBuilder>()))
			.Verifiable();

			services.AddSingleton(configuratorMock.Object);
			services.AddTransmitly<ICommunicationsClientConfigurator>();

			var provider = services.BuildServiceProvider();

			var client = provider.GetRequiredService<ICommunicationsClient>();
			client.DispatchAsync([]);
			// Assert - Verify ConfigureClient was called exactly once
			configuratorMock.Verify(c => c.ConfigureClient(It.IsAny<CommunicationsClientBuilder>()), Times.Once);
		}

		[TestMethod]
		public void AddTransmitlyGenericShouldCallConfigureClientOnResolution()
		{
			var services = new ServiceCollection();
			var tracker = new ConfigureClientTracker();

			services.AddSingleton(tracker);
			services.AddTransmitly<MockClientConfigurator>();

			var provider = services.BuildServiceProvider();

			// Act - Trigger service resolution
			var client = provider.GetRequiredService<ICommunicationsClient>();
			client.DispatchAsync([]);
			// Assert - Ensure ConfigureClient was called
			Assert.IsTrue(tracker.WasCalled, "ConfigureClient was not called on MockClientConfigurator.");
		}

		[TestMethod]
		public void InjectedClientShouldBeRegisteredAsync()
		{
			var configuratorMock = new Mock<ICommunicationsClientConfigurator>();
			configuratorMock.Setup(x => x.ConfigureClient(It.IsAny<CommunicationsClientBuilder>())).Verifiable();

			var services = new ServiceCollection();
			services.AddSingleton(configuratorMock.Object);
			services.AddTransmitly<ICommunicationsClientConfigurator, InjectedClient>();

			var provider = services.BuildServiceProvider();
			var client = provider.GetService<ICommunicationsClient>();

			Assert.IsNotNull(client);
			Assert.IsInstanceOfType<InjectedClient>(client);

		}

		[TestMethod]
		public async Task InjectedClientShouldNotCallConfigureClientMultipleTimes()
		{
			var configuratorMock = new Mock<ICommunicationsClientConfigurator>();
			configuratorMock
			.Setup(x => x.ConfigureClient(It.IsAny<CommunicationsClientBuilder>()))
			.Callback<CommunicationsClientBuilder>(builder =>
			{
				builder.AddPipeline("testPipeline", pipeline => { });
			})
			.Verifiable();

			var services = new ServiceCollection();
			services.AddSingleton(configuratorMock.Object);
			services.AddTransmitly<ICommunicationsClientConfigurator, InjectedClient>();
			//services.AddSingleton(configuratorMock.Object);

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync("testPipeline", new List<IPlatformIdentityProfile>(), TransactionModel.Create(new { }), new List<string>());
			await client.DispatchAsync("testPipeline", new List<IPlatformIdentityProfile>(), TransactionModel.Create(new { }), new List<string>());

			configuratorMock.Verify(x => x.ConfigureClient(It.IsAny<CommunicationsClientBuilder>()), Times.Once);
		}

		[TestMethod]
		public async Task DefaultClientShouldNotCallConfigureClientMultipleTimes()
		{
			var configuratorMock = new Mock<ICommunicationsClientConfigurator>();
			configuratorMock
			.Setup(x => x.ConfigureClient(It.IsAny<CommunicationsClientBuilder>()))
			.Callback<CommunicationsClientBuilder>(builder =>
			{
				builder.AddPipeline("testPipeline", pipeline => { });
			})
			.Verifiable();

			var services = new ServiceCollection();
			services.AddSingleton(configuratorMock.Object);
			services.AddTransmitly<ICommunicationsClientConfigurator>();
			//services.AddSingleton(configuratorMock.Object);

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync("testPipeline", new List<IPlatformIdentityProfile>(), TransactionModel.Create(new { }), new List<string>());
			await client.DispatchAsync("testPipeline", new List<IPlatformIdentityProfile>(), TransactionModel.Create(new { }), new List<string>());

			configuratorMock.Verify(x => x.ConfigureClient(It.IsAny<CommunicationsClientBuilder>()), Times.Once);
		}

		[TestMethod]
		public async Task ShouldUseDIForPlatformIdentityResolver()
		{
			var services = new ServiceCollection();
			var dependencyInstance = new SimpleDependency();
			IdentityResolverWithDependency.CapturedDependency = null;

			services.AddSingleton(dependencyInstance);
			services.AddTransient<IdentityResolverWithDependency>();

			services.AddTransmitly(tly =>
			{
				tly.ChannelProvider.Add<TestChannelProviderDispatcher2>("Test Channel")
				   .AddPlatformIdentityResolver<IdentityResolverWithDependency>()
				   .AddPipeline("testPipeline", pipeline =>
					   pipeline.AddEmail("test@test.com", email =>
					   {
						   email.Subject.AddStringTemplate("test-subject");
						   email.TextBody.AddStringTemplate("test-body");
					   }));
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync("testPipeline",
				new[] { new TestIdentityReference { Id = "test-id", Type = "test-type" } },
				TransactionModel.Create(new { }), Array.Empty<string>());

			Assert.AreSame(dependencyInstance, IdentityResolverWithDependency.CapturedDependency);
		}

		[TestMethod]
		public async Task ShouldUseDIForPlatformIdentityProfileEnricher()
		{
			var services = new ServiceCollection();
			var dependencyInstance = new SimpleDependency();
			ProfileEnricherWithDependency.CapturedDependency = null;

			services.AddSingleton(dependencyInstance);
			services.AddTransient<TestPlatformIdentityResolver>();
			services.AddTransient<ProfileEnricherWithDependency>();

			services.AddTransmitly(tly =>
			{
				tly.ChannelProvider.Add<TestChannelProviderDispatcher2>("Test Channel")
				   .AddPlatformIdentityResolver<TestPlatformIdentityResolver>()
				   .AddPlatformIdentityProfileEnricher<ProfileEnricherWithDependency>()
				   .AddPipeline("testPipeline", pipeline =>
					   pipeline.AddEmail("test@test.com", email =>
					   {
						   email.Subject.AddStringTemplate("test-subject");
						   email.TextBody.AddStringTemplate("test-body");
					   }));
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync("testPipeline",
				new[] { new TestIdentityReference { Id = "test-id", Type = "test-type" } },
				TransactionModel.Create(new { }), Array.Empty<string>());

			Assert.AreSame(dependencyInstance, ProfileEnricherWithDependency.CapturedDependency);
		}

		[TestMethod]
		public async Task ShouldFailDispatchWhenProfileEnricherIsNotRegisteredInDI()
		{
			var services = new ServiceCollection();
			services.AddTransient<SimpleDependency>();

			services.AddTransmitly(tly =>
			{
				ConfigureBasicPipeline(tly);
				tly.AddPlatformIdentityProfileEnricher<ProfileEnricherWithDependency>();
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			var result = await client.DispatchAsync(
				"testPipeline",
				new[] { CreateIdentityProfile() },
				TransactionModel.Create(new { }),
				Array.Empty<string>());

			Assert.AreEqual(1, result.Results.Count);
			Assert.IsTrue(result.Results.Single().Status.IsFailure());
		}

		[TestMethod]
		public async Task ShouldFailDispatchWhenProfileEnricherThrows()
		{
			var services = new ServiceCollection();
			services.AddTransient<SimpleDependency>();
			services.AddTransient<ThrowingProfileEnricher>();

			services.AddTransmitly(tly =>
			{
				ConfigureBasicPipeline(tly);
				tly.AddPlatformIdentityProfileEnricher<ThrowingProfileEnricher>();
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			var result = await client.DispatchAsync(
				"testPipeline",
				new[] { CreateIdentityProfile() },
				TransactionModel.Create(new { }),
				Array.Empty<string>());

			Assert.AreEqual(1, result.Results.Count);
			Assert.IsTrue(result.Results.Single().Status.IsFailure());
		}

		[TestMethod]
		public async Task ShouldApplyOnlyMatchingTypeScopedEnrichers()
		{
			var services = new ServiceCollection();
			var tracker = new ProfileEnricherTracker();

			services.AddTransient<SimpleDependency>();
			services.AddSingleton(tracker);
			services.AddTransient<TypeScopedGlobalProfileEnricher>();
			services.AddTransient<TypeScopedMemberProfileEnricher>();
			services.AddTransient<TypeScopedOtherProfileEnricher>();

			services.AddTransmitly(tly =>
			{
				ConfigureBasicPipeline(tly);
				tly.AddPlatformIdentityProfileEnricher<TypeScopedOtherProfileEnricher>("other-type", 10)
				   .AddPlatformIdentityProfileEnricher<TypeScopedGlobalProfileEnricher>(order: 20)
				   .AddPlatformIdentityProfileEnricher<TypeScopedMemberProfileEnricher>("member-type", 30);
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync(
				"testPipeline",
				new[] { CreateIdentityProfile(type: "member-type") },
				TransactionModel.Create(new { }),
				Array.Empty<string>());

			var execution = tracker.ExecutionOrder;
			Assert.AreEqual(2, execution.Count);
			Assert.IsTrue(execution.Contains("global"));
			Assert.IsTrue(execution.Contains("member"));
		}

		[TestMethod]
		public async Task ShouldExecuteProfileEnrichersInOrder()
		{
			var services = new ServiceCollection();
			var tracker = new ProfileEnricherTracker();

			services.AddTransient<SimpleDependency>();
			services.AddSingleton(tracker);
			services.AddTransient<OrderedFirstProfileEnricher>();
			services.AddTransient<OrderedSecondProfileEnricher>();
			services.AddTransient<OrderedThirdProfileEnricher>();

			services.AddTransmitly(tly =>
			{
				ConfigureBasicPipeline(tly);
				tly.AddPlatformIdentityProfileEnricher<OrderedThirdProfileEnricher>(order: 30)
				   .AddPlatformIdentityProfileEnricher<OrderedFirstProfileEnricher>(order: 10)
				   .AddPlatformIdentityProfileEnricher<OrderedSecondProfileEnricher>(order: 20);
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync(
				"testPipeline",
				new[] { CreateIdentityProfile() },
				TransactionModel.Create(new { }),
				Array.Empty<string>());

			CollectionAssert.AreEqual(
				new[] { "first", "second", "third" },
				tracker.ExecutionOrder.ToArray());
		}

		[TestMethod]
		public async Task ShouldEnrichEachIdentityProfileInCollection()
		{
			var services = new ServiceCollection();
			var tracker = new ProfileEnricherTracker();

			services.AddTransient<SimpleDependency>();
			services.AddSingleton(tracker);
			services.AddTransient<CountingProfileEnricher>();

			services.AddTransmitly(tly =>
			{
				ConfigureBasicPipeline(tly);
				tly.AddPlatformIdentityProfileEnricher<CountingProfileEnricher>();
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync(
				"testPipeline",
				new[]
				{
					CreateIdentityProfile(id: "1"),
					CreateIdentityProfile(id: "2"),
					CreateIdentityProfile(id: "3")
				},
				TransactionModel.Create(new { }),
				Array.Empty<string>());

			Assert.AreEqual(3, tracker.InvocationCount);
		}

		[TestMethod]
		public async Task ResolvedIdentityProfilesShouldAlsoBeEnriched()
		{
			var services = new ServiceCollection();
			var tracker = new ProfileEnricherTracker();

			services.AddTransient<SimpleDependency>();
			services.AddSingleton(tracker);
			services.AddTransient<TestPlatformIdentityResolver>();
			services.AddTransient<CountingProfileEnricher>();

			services.AddTransmitly(tly =>
			{
				ConfigureBasicPipeline(tly);
				tly.AddPlatformIdentityResolver<TestPlatformIdentityResolver>()
				   .AddPlatformIdentityProfileEnricher<CountingProfileEnricher>();
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();

			await client.DispatchAsync(
				"testPipeline",
				new[] { new TestIdentityReference { Id = "resolved-user", Type = "member-type" } },
				TransactionModel.Create(new { }),
				Array.Empty<string>());

			Assert.AreEqual(1, tracker.InvocationCount);
		}

		private static PlatformIdentityProfile CreateIdentityProfile(string id = "test-id", string type = "test-type")
		{
			return new PlatformIdentityProfile(id, type, new[] { "test@test.com".AsIdentityAddress() });
		}

		private static void ConfigureBasicPipeline(CommunicationsClientBuilder tly)
		{
			tly.ChannelProvider.Add<TestChannelProviderDispatcher2>("Test Channel")
			   .AddPipeline("testPipeline", pipeline =>
				   pipeline.AddEmail("test@test.com", email =>
				   {
					   email.Subject.AddStringTemplate("test-subject");
					   email.TextBody.AddStringTemplate("test-body");
				   }));
		}
	}
}
