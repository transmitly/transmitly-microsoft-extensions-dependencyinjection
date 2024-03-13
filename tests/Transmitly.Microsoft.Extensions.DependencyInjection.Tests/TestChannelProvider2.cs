using Transmitly.ChannelProvider;

namespace Transmitly.Microsoft.Extensions.DependencyInjection.Tests
{
	class TestChannelProvider2(SimpleDependency foo) : IChannelProviderClient<object>
	{
		public readonly SimpleDependency _foo = foo ?? throw new ArgumentNullException(nameof(foo));

		public IReadOnlyCollection<string>? RegisteredEvents => throw new NotImplementedException();

		public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(object communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
		{
			return Task.FromResult<IReadOnlyCollection<IDispatchResult?>>([new DispatchResult(DispatchStatus.Dispatched, "Object2")]);
		}
	}
}