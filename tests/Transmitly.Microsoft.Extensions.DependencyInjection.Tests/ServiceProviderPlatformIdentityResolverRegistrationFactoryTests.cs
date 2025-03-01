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
    public class ServiceProviderPlatformIdentityResolverRegistrationFactoryTests
    {
        [TestMethod]
        public async Task ResolveResolver_ShouldReturnResolverInstance_WhenRegistrationIsValid()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var resolverMock = new Mock<IPlatformIdentityResolver>();
            var registrationMock = new Mock<IPlatformIdentityResolverRegistration>();
            registrationMock.Setup(r => r.ResolverType).Returns(resolverMock.Object.GetType());
            serviceProviderMock.Setup(sp => sp.GetService(resolverMock.Object.GetType())).Returns(resolverMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(Castle.DynamicProxy.IInterceptor[]))).Returns(new Castle.DynamicProxy.IInterceptor[0]);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(object))).Returns(new object()); 

            var factory = new ServiceProviderPlatformIdentityResolverRegistrationFactory(
                new[] { registrationMock.Object },
                serviceProviderMock.Object
            );

            var result = await factory.ResolveResolver(registrationMock.Object);

            Assert.IsNotNull(result);
            Assert.AreEqual(resolverMock.Object.GetType(), result.GetType());
        }

        [TestMethod]
        public async Task ResolveResolver_ShouldThrowArgumentNullException_WhenRegistrationIsNull()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var factory = new ServiceProviderPlatformIdentityResolverRegistrationFactory(
                Array.Empty<IPlatformIdentityResolverRegistration>(),
                serviceProviderMock.Object
            );

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => factory.ResolveResolver(null));
        }
    }
}