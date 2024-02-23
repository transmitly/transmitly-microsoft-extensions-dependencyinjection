using System;
using Transmitly;
using Transmitly.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class TransmitlyDependencyInjectionExtensions
	{
		public static IServiceCollection AddTransmitly(this IServiceCollection services, Action<CommunicationsClientBuilder> options)
		{
			var builder = new CommunicationsClientBuilder();
			options(builder);
			builder.RegisterClientFactory(new ServiceCollectionCommunicationClientFactory(services));
			builder.BuildClient();
			return services;
		}
	}
}