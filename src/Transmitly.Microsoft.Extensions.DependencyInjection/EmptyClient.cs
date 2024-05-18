using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Transmitly.Delivery;
using Transmitly.Verification;

namespace Transmitly.Microsoft.Extensions.DependencyInjection
{
	sealed class EmptyClient : ICommunicationsClient
	{
		public void DeliverReport(DeliveryReport report)
		{
			throw new NotImplementedException();
		}

		public void DeliverReports(IReadOnlyCollection<DeliveryReport> reports)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IAudience> audiences, IContentModel contentModel, IReadOnlyCollection<string> allowedChannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IAudienceAddress> audienceAddresses, IContentModel contentModel, IReadOnlyCollection<string> allowedChannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, string audienceAddress, IContentModel contentModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, string audienceAddress, object contentModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<string> audienceAddresses, object contentModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IAudienceAddress> audienceAddresses, IContentModel contentModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IAudience> audiences, IContentModel contentModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

	}
}