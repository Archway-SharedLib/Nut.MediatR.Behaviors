using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.Behaviors.Test.RequestAware;

public class RequestAwareBehaviorBuilderTest
{
    [Fact]
    public void AddOpenBehavior_OpenBehaviorのタイプが登録される()
    {
        var services = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(services);
        var openBehaviorType = typeof(TestBehavior<>);

        builder.AddOpenBehavior(openBehaviorType);
        builder.Build();

        builder.Services.Should().ContainSingle(d =>
            d.ServiceType == typeof(TestBehavior<>) &&
            d.ImplementationType == typeof(TestBehavior<>));
    }

    [Fact]
    public void AddOpenBehavior_OpenBehaviorで無い場合は例外が発生する()
    {
        var services = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(services);
        var nonGenericBehaviorType = typeof(NonGenericBehavior);

        var action = () => builder.AddOpenBehavior(nonGenericBehaviorType);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddOpenBehavior_IPipelineBehaviorを継承していないと例外が発生する()
    {
        var services = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(services);
        var behaviorType = typeof(InvalidBehavior<>);

        Action action = () => builder.AddOpenBehavior(behaviorType);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Build_AutoRegistrationHandlerが呼ばれるべき()
    {
        var services = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(services);
        var assemblies = new[] { Assembly.GetExecutingAssembly() };
        var handlerCalled = false;
        void handler(IServiceCollection s, Assembly[] a)
        {
            handlerCalled = true;
            s.Should().BeSameAs(builder.Services);
            a.Should().BeEquivalentTo(assemblies);
        }
        builder.AddAutoRegistrationHandler(handler);
        builder.AddAssembliesForAutoRegister(assemblies);

        builder.Build();

        handlerCalled.Should().BeTrue();
    }

    public class TestBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
    {
        public Task<Unit> Handle(TRequest request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    public class NonGenericBehavior : IPipelineBehavior<object, Unit>
    {
        public Task<Unit> Handle(object request, RequestHandlerDelegate<Unit> next, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    public class InvalidBehavior<TRequest>
    {
        public System.Threading.Tasks.Task<Unit> Handle(TRequest request, System.Threading.CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            throw new NotImplementedException();
        }
    }
}
