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

using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Transmitly;
using Transmitly.Microsoft.Extensions.DependencyInjection;
using Transmitly.Util;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class TransmitlyDependencyInjectionExtensions
	{
		public static IServiceCollection AddTransmitly(this IServiceCollection services, Type configuratorType)
		{
			ValidateImplementsInterface(configuratorType, typeof(ICommunicationsClientConfigurator));

			services.TryAddSingleton(typeof(ICommunicationsClientConfigurator), configuratorType);
			services.AddSingleton<ICommunicationsClient, ServiceCollectionCommunicationClient>();
			return services;
		}

		public static IServiceCollection AddTransmitly<TConfigurator>(this IServiceCollection services)
			where TConfigurator : class, ICommunicationsClientConfigurator
		{
			return AddTransmitly(services, typeof(TConfigurator));
		}

		public static IServiceCollection AddTransmitly<TConfigurator, TClient>(this IServiceCollection services, Type configuratorType, Type clientType)
		{
			Guard.AgainstNull(services);

			ValidateImplementsInterface(configuratorType, typeof(ICommunicationsClientConfigurator));
			ValidateImplementsInterface(clientType, typeof(ICommunicationsClient));

			services.TryAddSingleton(typeof(ICommunicationsClientConfigurator), configuratorType);
			services.AddSingleton(typeof(ICommunicationsClient), clientType);
			return services;
		}

		public static IServiceCollection AddTransmitly<TConfigurator, TClient>(this IServiceCollection services)
			where TConfigurator : class, ICommunicationsClientConfigurator
			where TClient : class, ICommunicationsClient
		{
			return AddTransmitly<TConfigurator, TClient>(services, typeof(TConfigurator), typeof(TClient));
		}

		public static IServiceCollection AddTransmitly(this IServiceCollection services, Action<CommunicationsClientBuilder> options)
		{
			Guard.AgainstNull(options);
			Guard.AgainstNull(services);

			var builder = new CommunicationsClientBuilder();
			options(builder);
			builder.AddClientMiddleware(new ServiceCollectionCommunicationClientFactory(services));
			builder.BuildClient();
			return services;
		}

		private static void ValidateImplementsInterface(Type type, Type interfaceType)
		{
			Guard.AgainstNull(type);
			Guard.AgainstNull(interfaceType);

			if (!interfaceType.IsAssignableFrom(type))
				throw new InvalidOperationException($"Type {type.FullName} must implement {interfaceType.FullName}");
		}
	}
}