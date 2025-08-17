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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Transmitly;

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
		public async Task ShouldRegisterMiddlewareClientAsync()
		{
			var injectedMiddleware = new Mock<ICommunicationClientMiddleware>();
			var injectedClient = new Mock<ICommunicationsClient>();
			injectedMiddleware.Setup(x => x.CreateClient(It.IsAny<ICreateCommunicationsClientContext>(), It.IsAny<ICommunicationsClient?>()))
				.Returns(injectedClient.Object);
			
			var services = new ServiceCollection();

			services.AddTransmitly(tly =>
			{
				tly.RegisterClientMiddleware(injectedMiddleware.Object)
					.AddPipeline("testPipeline", pipeline => { });
			});

			var provider = services.BuildServiceProvider();
			var client = provider.GetRequiredService<ICommunicationsClient>();
			Assert.AreSame(injectedClient.Object, client);
		}
	}
}