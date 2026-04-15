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

using Moq;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Delivery;

namespace Transmitly.Microsoft.Extensions.DependencyInjection.Tests
{
	[TestClass]
	public class ServiceProviderChannelProviderFactoryTests
	{
		[TestMethod]
		public async Task ResolveDispatcherAsync_ShouldReturnRegisteredDispatcher_WhenAvailable()
		{
			var serviceProvider = new Mock<IServiceProvider>();
			var dispatcher = new Mock<IChannelProviderDispatcher>();
			var channelProviderRegistration = new Mock<IChannelProviderRegistration>();
			var dispatcherRegistration = new Mock<IChannelProviderDispatcherRegistration>();
			dispatcherRegistration.SetupGet(x => x.DispatcherType).Returns(dispatcher.Object.GetType());
			serviceProvider.Setup(x => x.GetService(dispatcher.Object.GetType())).Returns(dispatcher.Object);

			var factory = new ServiceProviderChannelProviderFactory([channelProviderRegistration.Object], serviceProvider.Object);

			var resolved = await factory.ResolveDispatcherAsync(channelProviderRegistration.Object, dispatcherRegistration.Object);

			Assert.AreSame(dispatcher.Object, resolved);
		}

		[TestMethod]
		public async Task ResolveDispatcherAsync_ShouldReturnNull_WhenDispatcherIsNotRegistered()
		{
			var serviceProvider = new Mock<IServiceProvider>();
			var channelProviderRegistration = new Mock<IChannelProviderRegistration>();
			var dispatcherRegistration = new Mock<IChannelProviderDispatcherRegistration>();
			dispatcherRegistration.SetupGet(x => x.DispatcherType).Returns(typeof(TestChannelProviderDispatcher2));

			var factory = new ServiceProviderChannelProviderFactory([channelProviderRegistration.Object], serviceProvider.Object);

			var resolved = await factory.ResolveDispatcherAsync(channelProviderRegistration.Object, dispatcherRegistration.Object);

			Assert.IsNull(resolved);
		}

		[TestMethod]
		public async Task ResolveDeliveryReportRequestAdaptorAsync_ShouldReturnRegisteredAdaptor_WhenAvailable()
		{
			var serviceProvider = new Mock<IServiceProvider>();
			var adaptor = new Mock<IChannelProviderDeliveryReportRequestAdaptor>();
			var adaptorRegistration = new Mock<IDeliveryReportRequestAdaptorRegistration>();
			adaptorRegistration.SetupGet(x => x.Type).Returns(adaptor.Object.GetType());
			serviceProvider.Setup(x => x.GetService(adaptor.Object.GetType())).Returns(adaptor.Object);

			var factory = new ServiceProviderChannelProviderFactory([], serviceProvider.Object);

			var resolved = await factory.ResolveDeliveryReportRequestAdaptorAsync(adaptorRegistration.Object);

			Assert.AreSame(adaptor.Object, resolved);
		}

		[TestMethod]
		public async Task ResolveDeliveryReportRequestAdaptorAsync_ShouldThrow_WhenAdaptorIsNotRegistered()
		{
			var serviceProvider = new Mock<IServiceProvider>();
			var adaptorRegistration = new Mock<IDeliveryReportRequestAdaptorRegistration>();
			adaptorRegistration.SetupGet(x => x.Type).Returns(typeof(TestDeliveryReportRequestAdaptor));

			var factory = new ServiceProviderChannelProviderFactory([], serviceProvider.Object);

			await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => factory.ResolveDeliveryReportRequestAdaptorAsync(adaptorRegistration.Object));
		}
	}
}
