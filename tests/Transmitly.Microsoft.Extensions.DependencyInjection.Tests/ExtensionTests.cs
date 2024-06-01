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
				tly.ChannelProvider.Add<TestChannelProvider, object>("test-channel-provider");
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
				tly.ChannelProvider.Add<TestChannelProvider, ISms>("test-channel-provider");
				tly.ChannelProvider.Add<TestChannelProvider2, IEmail>("test-channel-provider");
				tly.ChannelProvider.Add<TestChannelProvider, IEmail>("test-channel-provider");

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
	}
}