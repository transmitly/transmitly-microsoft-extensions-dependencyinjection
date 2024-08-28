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

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentity> platformIdentities, ITransactionModel transactionalModel, IReadOnlyCollection<string> allowedChannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityAddress> identityAddresses, ITransactionModel transactionalModel, IReadOnlyCollection<string> allowedChannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, string identityAddress, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, string identityAddress, object transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<string> identityAddresses, object transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityAddress> identityAddresses, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IPlatformIdentity> platformIdentities, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityReference> identityReferences, ITransactionModel transactionalModel, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IDispatchCommunicationResult> DispatchAsync(string pipelineName, IReadOnlyCollection<IIdentityReference> identityReferences, ITransactionModel transactionalModel, IReadOnlyCollection<string> allowedCHannels, string? cultureInfo = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}