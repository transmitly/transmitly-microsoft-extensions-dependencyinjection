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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Transmitly.ChannelProvider;

namespace Transmitly.Microsoft.Extensions.DependencyInjection.Tests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public async Task ShouldRegisterCommunicationClient()
        {
            var services = new ServiceCollection();
            services.AddTransient<SimpleDependency>();

            services.AddTransmitly(tly =>
            {
                tly.ChannelProvider.Add<TestChannelProviderDispatcher, object>("test-channel-provider");
                tly.Pipeline.Add("test-pipeline", options =>
                {
                    options.AddEmail("from@address.com".AsIdentityAddress(), email =>
                    {
                        email.Subject.AddStringTemplate("Test sub");
                    });
                });
            });

            var provider = services.BuildServiceProvider();
            var client = provider.GetService<ICommunicationsClient>();
            Assert.IsNotNull(client);
            var result = await client.DispatchAsync("test-pipeline", "test@test.com", new { });
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(Id.Channel.Email(), result.Results.FirstOrDefault()?.ChannelId);
            Assert.AreEqual("test-channel-provider", result.Results.FirstOrDefault()?.ChannelProviderId);
        }

        [TestMethod]
        public async Task ShouldResolveCorrectClient()
        {
            var services = new ServiceCollection();
            services.AddTransient<SimpleDependency>();

            services.AddTransmitly(tly =>
            {
                tly.ChannelProvider.Add<TestChannelProviderDispatcher, ISms>("test-channel-provider");
                tly.ChannelProvider.Add<TestChannelProviderDispatcher2, IEmail>("test-channel-provider");
                tly.ChannelProvider.Add<TestChannelProviderDispatcher, IEmail>("test-channel-provider");

                tly.Pipeline.Add("test-pipeline", options =>
                {
                    options.AddEmail("from@address.com".AsIdentityAddress(), email =>
                    {
                        email.Subject.AddStringTemplate("Test sub");

                    });
                    options.UseAnyMatchPipelineDeliveryStrategy();
                });
            });

            var provider = services.BuildServiceProvider();
            var client = provider.GetService<ICommunicationsClient>();
            Assert.IsNotNull(client);
            var result = await client.DispatchAsync("test-pipeline", "test@test.com", new { });
            Assert.AreEqual(2, result.Results.Count);
            var results = result.Results.ToList();
            Assert.AreEqual("Object2", results[0]?.ResourceId);
            Assert.AreEqual("IEmail", results[1]?.ResourceId);
        }

        [TestMethod]
        public async Task ShouldRegisterPlatformIdentityResolverFactory()
        {
            var services = new ServiceCollection();
            services.AddTransient<SimpleDependency>();

            services.AddTransmitly(tly =>
            {
                tly.AddPlatformIdentityResolver<TestPlatformIdentityResolver>();
                tly.ChannelProvider.Add<TestChannelProviderDispatcher, object>("test-channel-provider");
                tly.Pipeline.Add("test-pipeline", options =>
                {
                    options.AddEmail("from@address.com".AsIdentityAddress(), email =>
                    {
                        email.Subject.AddStringTemplate("Test sub");
                    });
                });
            });

            var provider = services.BuildServiceProvider();
            var client = provider.GetService<ICommunicationsClient>();
            Assert.IsNotNull(client);
            var result = await client.DispatchAsync("test-pipeline", new List<TestIdentityReference> { new TestIdentityReference { Id = "Test", Type = "" } }, TransactionModel.Create(new { }));
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(Id.Channel.Email(), result.Results.FirstOrDefault()?.ChannelId);
            Assert.AreEqual("test-channel-provider", result.Results.FirstOrDefault()?.ChannelProviderId);
        }
    }
}