using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Linq;

namespace Nut.MediatR.Test;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddMediatRRequestAwareBehavior_RequestAwareBehaviorが登録されるべき()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddMediatRRequestAwareBehavior(builder =>
        {
            builder.AddOpenBehavior(typeof(BehaviorForTest<,>));
        });

        serviceCollection.Count(descriptor =>
            descriptor.ServiceType == typeof(IPipelineBehavior<,>) &&
            descriptor.ImplementationType == typeof(RequestAwareBehavior<,>)).ShouldBe(1);

        serviceCollection.Count(descriptor =>
            descriptor.ServiceType == typeof(BehaviorForTest<,>) &&
            descriptor.ImplementationType == typeof(BehaviorForTest<,>)).ShouldBe(1);
    }

    public class BehaviorForTest<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}
