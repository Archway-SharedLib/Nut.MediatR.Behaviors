using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.Test.PerRequest;

public class PerRequestBehaviorTest
{
    [Fact]
    public void ctor_引数がnullの場合は例外が発生する()
    {
        Action act = () => new PerRequestBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_WithBehaviorAttributeに設定されているBehaviorが実行される()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(PerRequestBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(PerRequestBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior1<,>));
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new Req1());

        var list = provider.GetService<ExecHistory>().List;

        list.Count.Should().Be(6);
        list[0].Should().Be(TestBehaviorMessages.StartMessage3);
        list[1].Should().Be(TestBehaviorMessages.StartMessage1);
        list[2].Should().Be(TestBehaviorMessages.StartMessage2);
        list[3].Should().Be(TestBehaviorMessages.EndMessage2);
        list[4].Should().Be(TestBehaviorMessages.EndMessage1);
        list[5].Should().Be(TestBehaviorMessages.EndMessage3);
    }

    [Fact]
    public async Task Handle_WithBehaviorsが設定されていない場合は何も実行されない()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(PerRequestBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(PerRequestBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior1<,>));
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new Req2());

        var list = provider.GetService<ExecHistory>().List;
        list.Count.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceFactoryからインスタンスが返されないBehaviorはスキップされる()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(PerRequestBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(PerRequestBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new Req1());

        var list = provider.GetService<ExecHistory>().List;

        list.Count.Should().Be(4);
        list[0].Should().Be(TestBehaviorMessages.StartMessage3);
        list[1].Should().Be(TestBehaviorMessages.StartMessage2);
        list[2].Should().Be(TestBehaviorMessages.EndMessage2);
        list[3].Should().Be(TestBehaviorMessages.EndMessage3);
    }

    [Fact]
    public async Task Handle_Void_WithBehaviorAttributeに設定されているBehaviorが実行される()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(PerRequestBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(PerRequestBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior1<,>));
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        await mediator.Send(new Req3());

        var list = provider.GetService<ExecHistory>().List;

        list.Count.Should().Be(6);
        list[0].Should().Be(TestBehaviorMessages.StartMessage3);
        list[1].Should().Be(TestBehaviorMessages.StartMessage1);
        list[2].Should().Be(TestBehaviorMessages.StartMessage2);
        list[3].Should().Be(TestBehaviorMessages.EndMessage2);
        list[4].Should().Be(TestBehaviorMessages.EndMessage1);
        list[5].Should().Be(TestBehaviorMessages.EndMessage3);
    }
}

public class Res
{
}

[WithBehaviors(typeof(TestBehavior3<,>), typeof(TestBehavior1<,>), typeof(TestBehavior2<,>))]
public class Req1 : IRequest<Res>
{
}

public class Req1Handler : IRequestHandler<Req1, Res>
{
    public Task<Res> Handle(Req1 request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Res());
    }
}

public class Req2 : IRequest<Res>
{
}

public class Req2Handler : IRequestHandler<Req2, Res>
{
    public Task<Res> Handle(Req2 request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Res());
    }
}

[WithBehaviors(typeof(TestBehavior3<,>), typeof(TestBehavior1<,>), typeof(TestBehavior2<,>))]
public class Req3 : IRequest
{
}

public class Req3Handler : IRequestHandler<Req3>
{
    public Task Handle(Req3 request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
