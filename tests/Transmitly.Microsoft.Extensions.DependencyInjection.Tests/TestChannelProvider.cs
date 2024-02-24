using Transmitly.ChannelProvider;

namespace Transmitly.Microsoft.Extensions.DependencyInjection.Tests
{
	class TestChannelProvider : IChannelProviderClient<object>, IChannelProviderClient<IEmail>
	{
		public readonly SimpleDependency _foo;

		public TestChannelProvider(SimpleDependency foo)
		{
			_foo = foo ?? throw new ArgumentNullException(nameof(foo));
		}
		public IReadOnlyCollection<string>? RegisteredEvents => throw new NotImplementedException();

		public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(object communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
		{
			return Task.FromResult<IReadOnlyCollection<IDispatchResult?>>([new DispatchResult(true, "Object")]);
		}

		public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(IEmail communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
		{
			return Task.FromResult<IReadOnlyCollection<IDispatchResult?>>([new DispatchResult(true, "IEmail")]);
		}
	}

	class TestChannelProvider2 : IChannelProviderClient<object>
	{
		public readonly SimpleDependency _foo;

		public TestChannelProvider2(SimpleDependency foo)
		{
			_foo = foo ?? throw new ArgumentNullException(nameof(foo));
		}
		public IReadOnlyCollection<string>? RegisteredEvents => throw new NotImplementedException();

		public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(object communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
		{
			return Task.FromResult<IReadOnlyCollection<IDispatchResult?>>([new DispatchResult(true, "Object2")]);
		}
	}
}