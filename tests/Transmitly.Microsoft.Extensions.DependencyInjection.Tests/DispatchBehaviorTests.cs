// Copyright (c) Code Impressions, LLC. All Rights Reserved.
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
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Delivery;
using Transmitly.Microsoft.Extensions.DependencyInjection.Tests;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
	public partial class TransmitlyDependencyInjectionExtensionsTests
	{
		[TestMethod]
		public async Task ShouldDispatchThroughTypedDispatcherOverloadForChannelCommunication()
		{
			var services = new ServiceCollection();
			services.AddTransient<SimpleDependency>();
			services.AddTransmitly(tly => tly
				.ChannelProvider.Add<TestChannelProviderDispatcher, IEmail>("email-provider")
				.AddPipeline("testPipeline", pipeline => pipeline.AddEmail("test@test.com", email => email.Subject.AddStringTemplate("test-subject"))));

			var result = await DispatchTestPipelineAsync(services.BuildServiceProvider());

			Assert.IsTrue(result.IsSuccessful);
			Assert.AreEqual("IEmail", result.Results.Single()?.ResourceId);
		}

		[TestMethod]
		[DataRow(false, DisplayName = "FirstMatch")]
		[DataRow(true, DisplayName = "AnyMatch")]
		public async Task ShouldFallThroughToNextProviderWhenDispatcherThrows(bool useAnyMatch)
		{
			var services = new ServiceCollection();
			services.AddTransmitly(tly => tly
				.ChannelProvider.Add<ThrowingDispatcher, object>("throwing-provider")
				.ChannelProvider.Add<SucceedingDispatcher, object>("succeeding-provider")
				.AddPipeline("testPipeline", pipeline =>
				{
					pipeline.AddEmail("test@test.com", email => email.Subject.AddStringTemplate("test-subject"));
					if (useAnyMatch)
						pipeline.UseAnyMatchPipelineDeliveryStrategy();
				}));

			var result = await DispatchTestPipelineAsync(services.BuildServiceProvider());

			Assert.IsTrue(result.IsSuccessful);
			CollectionAssert.AreEqual(new[] { "throwing-provider", "succeeding-provider" }, result.Results.Select(r => r?.ChannelProviderId).ToArray());
			Assert.IsInstanceOfType<InvalidOperationException>(result.Results.First()?.Exception);
		}

		[TestMethod]
		public async Task ShouldRaiseErrorDeliveryReportWhenDispatcherThrows()
		{
			var reported = new TaskCompletionSource<DeliveryReport>();
			var services = new ServiceCollection();
			services.AddTransmitly(tly => tly
				.AddDeliveryReportHandler(report =>
				{
					reported.TrySetResult(report);
					return Task.CompletedTask;
				}, [DeliveryReport.Event.Error()])
				.ChannelProvider.Add<ThrowingDispatcher, object>("throwing-provider")
				.AddPipeline("testPipeline", pipeline => pipeline.AddEmail("test@test.com", email => email.Subject.AddStringTemplate("test-subject"))));

			var result = await DispatchTestPipelineAsync(services.BuildServiceProvider());

			Assert.IsFalse(result.IsSuccessful);
			Assert.AreSame(reported.Task, await Task.WhenAny(reported.Task, Task.Delay(TimeSpan.FromSeconds(5), TestContext.CancellationTokenSource.Token)));
			Assert.AreEqual("throwing-provider", reported.Task.Result.ChannelProviderId);
			Assert.IsInstanceOfType<InvalidOperationException>(reported.Task.Result.Exception);
		}

		[TestMethod]
		public async Task ShouldReturnFailedResultWhenDispatcherDependencyIsNotRegistered()
		{
			// SimpleDependency is intentionally not registered
			var services = new ServiceCollection();
			services.AddTransmitly(ConfigureBasicPipeline);

			var result = await DispatchTestPipelineAsync(services.BuildServiceProvider());

			Assert.IsFalse(result.IsSuccessful);
			Assert.IsInstanceOfType<InvalidOperationException>(result.Results.Single()?.Exception);
		}

		[TestMethod]
		public async Task ShouldResolveEachProvidersOwnConfigurationWhenProvidersShareADispatcherType()
		{
			var configA = new DispatcherConfiguration("a");
			var configB = new DispatcherConfiguration("b");
			var services = new ServiceCollection();
			services.AddTransient<SimpleDependency>();
			services.AddTransmitly(tly => tly
				.ChannelProvider.Add<ConfiguredDispatcher, object>("provider-a", configA)
				.ChannelProvider.Add<ConfiguredDispatcher, object>("provider-b", configB));

			var factory = services.BuildServiceProvider().GetRequiredService<IChannelProviderFactory>();
			var registrations = await factory.GetAllAsync();

			async Task<DispatcherConfiguration?> ResolveConfiguration(string providerId)
			{
				var registration = registrations.Single(r => r.Id == providerId);
				var dispatcher = await factory.ResolveDispatcherAsync(registration, registration.DispatcherRegistrations.Single());
				return (dispatcher as ConfiguredDispatcher)?.Configuration;
			}

			Assert.AreSame(configA, await ResolveConfiguration("provider-a"));
			Assert.AreSame(configB, await ResolveConfiguration("provider-b"));
		}

		[TestMethod]
		public async Task ConfiguredDispatcherShouldBeReusedPerProviderAndDisposedWithContainer()
		{
			var services = new ServiceCollection();
			services.AddTransient<SimpleDependency>();
			services.AddTransmitly(tly => tly.ChannelProvider.Add<ConfiguredDispatcher, object>("provider-a", new DispatcherConfiguration("a")));
			var provider = services.BuildServiceProvider();
			var factory = provider.GetRequiredService<IChannelProviderFactory>();
			var registration = (await factory.GetAllAsync()).Single();

			var first = await factory.ResolveDispatcherAsync(registration, registration.DispatcherRegistrations.Single());
			var second = await factory.ResolveDispatcherAsync(registration, registration.DispatcherRegistrations.Single());
			provider.Dispose();

			Assert.AreSame(first, second);
			Assert.IsTrue(((ConfiguredDispatcher)first!).IsDisposed);
		}

		[TestMethod]
		public async Task ShouldPassContainerValidationAndDispatch()
		{
			var services = new ServiceCollection();
			services.AddTransient<SimpleDependency>();
			services.AddTransient<TestPlatformIdentityResolver>();
			services.AddTransient<ProfileEnricherWithDependency>();
			services.AddTransient<ContentModelEnricherWithDependency>();
			services.AddTransmitly(tly =>
			{
				var providerBuilder = tly.ChannelProvider.Build("Test Channel");
				providerBuilder.AddDispatcher<TestChannelProviderDispatcher2, object>();
				providerBuilder.AddDeliveryReportRequestAdaptor<TestDeliveryReportRequestAdaptor>();
				providerBuilder.Register();

				tly.AddPlatformIdentityResolver<TestPlatformIdentityResolver>()
				   .AddPlatformIdentityProfileEnricher<ProfileEnricherWithDependency>()
				   .AddContentModelEnricher<ContentModelEnricherWithDependency>()
				   .AddPipeline("testPipeline", pipeline => pipeline.AddEmail("test@test.com", email => email.Subject.AddStringTemplate("test-subject")));
			});

			// ASP.NET Core enables both of these in the Development environment
			using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

			var result = await DispatchTestPipelineAsync(provider);

			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ShouldResolveSameClientAcrossScopes()
		{
			var services = new ServiceCollection();
			services.AddTransient<SimpleDependency>();
			services.AddTransmitly(ConfigureBasicPipeline);
			using var provider = services.BuildServiceProvider();
			using var scope = provider.CreateScope();

			Assert.AreSame(provider.GetRequiredService<ICommunicationsClient>(), scope.ServiceProvider.GetRequiredService<ICommunicationsClient>());
		}

		[TestMethod]
		public async Task ConfiguratorClientShouldDispatchAndHonorChannelPreferences()
		{
			var services = new ServiceCollection();
			services.AddTransmitly<EmailAndSmsConfigurator>();
			var provider = services.BuildServiceProvider();

			var defaultResult = await DispatchTestPipelineAsync(provider);
			var smsResult = await DispatchTestPipelineAsync(provider, Id.Channel.Sms());

			Assert.AreEqual(Id.Channel.Email(), defaultResult.Results.Single()?.ChannelId);
			Assert.AreEqual(Id.Channel.Sms(), smsResult.Results.Single()?.ChannelId);
		}

		[TestMethod]
		public void AddTransmitlyShouldRejectTypesThatDoNotImplementTheExpectedInterface()
		{
			var services = new ServiceCollection();

			Assert.ThrowsExactly<InvalidOperationException>(() => services.AddTransmitly(typeof(object)));
			Assert.ThrowsExactly<InvalidOperationException>(() => services.AddTransmitly<MockClientConfigurator, InjectedClient>(typeof(MockClientConfigurator), typeof(object)));
		}

		[TestMethod]
		public void AddTransmitlyShouldRejectNullOptions()
		{
			Assert.ThrowsExactly<ArgumentNullException>(() => new ServiceCollection().AddTransmitly((Action<CommunicationsClientBuilder>)null!));
		}

		private Task<IDispatchCommunicationResult> DispatchTestPipelineAsync(IServiceProvider provider, params string[] channelPreferences)
		{
			return provider.GetRequiredService<ICommunicationsClient>().DispatchAsync(
				"testPipeline",
				new[] { new PlatformIdentityProfile("test-id", "test-type", new[] { "test@test.com".AsIdentityAddress(), "+15551231234".AsIdentityAddress() }) },
				TransactionModel.Create(new { }),
				channelPreferences,
				cancellationToken: TestContext.CancellationTokenSource.Token);
		}

		private sealed class SucceedingDispatcher : IChannelProviderDispatcher<object>
		{
			public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(object communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
			{
				return Task.FromResult<IReadOnlyCollection<IDispatchResult?>>([new DispatchResult(CommunicationsStatus.Success("succeeding-dispatcher", "ok"))]);
			}
		}

		private sealed class ThrowingDispatcher : IChannelProviderDispatcher<object>
		{
			public async Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(object communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
			{
				await Task.Yield();
				throw new InvalidOperationException("Intentional test exception");
			}
		}

		private sealed class EmailAndSmsConfigurator : ICommunicationsClientConfigurator
		{
			public void ConfigureClient(CommunicationsClientBuilder builder)
			{
				builder.ChannelProvider.Add<SucceedingDispatcher, object>("succeeding-provider")
					.AddPipeline("testPipeline", pipeline => pipeline
						.AddEmail("test@test.com", email => email.Subject.AddStringTemplate("test-subject"))
						.AddSms(sms => sms.Message.AddStringTemplate("test-sms")));
			}
		}
	}
}
