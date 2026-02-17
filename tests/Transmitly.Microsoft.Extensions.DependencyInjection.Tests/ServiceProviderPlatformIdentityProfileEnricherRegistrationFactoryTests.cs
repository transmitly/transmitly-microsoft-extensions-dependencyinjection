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
using Transmitly.PlatformIdentity.Configuration;

namespace Transmitly.Microsoft.Extensions.DependencyInjection.Tests
{
	[TestClass]
	public class ServiceProviderPlatformIdentityProfileEnricherRegistrationFactoryTests
	{
		[TestMethod]
		public async Task GetPlatformIdentityProfileEnricher_ShouldReturnEnricherInstance_WhenRegistrationIsValid()
		{
			var serviceProviderMock = new Mock<IServiceProvider>();
			var enricherMock = new Mock<IPlatformIdentityProfileEnricher>();
			var registrationMock = new Mock<IPlatformIdentityProfileEnricherRegistration>();
			registrationMock.Setup(r => r.EnricherType).Returns(enricherMock.Object.GetType());
			serviceProviderMock.Setup(sp => sp.GetService(enricherMock.Object.GetType())).Returns(enricherMock.Object);
			serviceProviderMock.Setup(sp => sp.GetService(typeof(Castle.DynamicProxy.IInterceptor[]))).Returns(Array.Empty<Castle.DynamicProxy.IInterceptor>());
			serviceProviderMock.Setup(sp => sp.GetService(typeof(object))).Returns(new object());

			var factory = new ServiceProviderPlatformIdentityProfileEnricherRegistrationFactory(
				new[] { registrationMock.Object },
				serviceProviderMock.Object
			);

			var result = await factory.GetPlatformIdentityProfileEnricher(registrationMock.Object);

			Assert.IsNotNull(result);
			Assert.AreEqual(enricherMock.Object.GetType(), result.GetType());
		}

		[TestMethod]
		public async Task GetPlatformIdentityProfileEnricher_ShouldThrowArgumentNullException_WhenRegistrationIsNull()
		{
			var serviceProviderMock = new Mock<IServiceProvider>();
			var factory = new ServiceProviderPlatformIdentityProfileEnricherRegistrationFactory(
				Array.Empty<IPlatformIdentityProfileEnricherRegistration>(),
				serviceProviderMock.Object
			);

			await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => factory.GetPlatformIdentityProfileEnricher(null!));
		}
	}
}
