using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.Test.RequestAware;

public class RequestAwareBehaviorTest
{
    [Fact]
    public void ctor_引数がnullの場合は例外が発生する()
    {
        Action act = () => new RequestAwareBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        Should.Throw<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Handle_WithBehaviorAttributeに設定されているBehaviorが実行される()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(RequestAwareBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior1<,>));
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new Req1());

        var list = provider.GetService<ExecHistory>().List;

        list.Count.ShouldBe(6);
        list[0].ShouldBe(TestBehaviorMessages.StartMessage3);
        list[1].ShouldBe(TestBehaviorMessages.StartMessage1);
        list[2].ShouldBe(TestBehaviorMessages.StartMessage2);
        list[3].ShouldBe(TestBehaviorMessages.EndMessage2);
        list[4].ShouldBe(TestBehaviorMessages.EndMessage1);
        list[5].ShouldBe(TestBehaviorMessages.EndMessage3);
    }

    [Fact]
    public async Task Handle_WithBehaviorsが設定されていない場合は何も実行されない()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(RequestAwareBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior1<,>));
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new Req2());

        var list = provider.GetService<ExecHistory>().List;
        list.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ServiceFactoryからインスタンスが返されないBehaviorはスキップされる()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(RequestAwareBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new Req1());

        var list = provider.GetService<ExecHistory>().List;

        list.Count.ShouldBe(4);
        list[0].ShouldBe(TestBehaviorMessages.StartMessage3);
        list[1].ShouldBe(TestBehaviorMessages.StartMessage2);
        list[2].ShouldBe(TestBehaviorMessages.EndMessage2);
        list[3].ShouldBe(TestBehaviorMessages.EndMessage3);
    }

    [Fact]
    public async Task Handle_Void_WithBehaviorAttributeに設定されているBehaviorが実行される()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(RequestAwareBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior1<,>));
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        await mediator.Send(new Req3());

        var list = provider.GetService<ExecHistory>().List;

        list.Count.ShouldBe(6);
        list[0].ShouldBe(TestBehaviorMessages.StartMessage3);
        list[1].ShouldBe(TestBehaviorMessages.StartMessage1);
        list[2].ShouldBe(TestBehaviorMessages.StartMessage2);
        list[3].ShouldBe(TestBehaviorMessages.EndMessage2);
        list[4].ShouldBe(TestBehaviorMessages.EndMessage1);
        list[5].ShouldBe(TestBehaviorMessages.EndMessage3);
    }

    [Fact]
    public async Task Handle_WithBehaviorAttributeを継承した属性の場合もBehaviorが実行される()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(RequestAwareBehavior<,>));
        });
        collection.AddTransient(typeof(TestBehavior1<,>));
        collection.AddTransient(typeof(TestBehavior2<,>));
        collection.AddTransient(typeof(TestBehavior3<,>));
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new Req4());

        var list = provider.GetService<ExecHistory>().List;

        list.Count.ShouldBe(4);
        list[0].ShouldBe(TestBehaviorMessages.StartMessage3);
        list[1].ShouldBe(TestBehaviorMessages.StartMessage1);
        list[2].ShouldBe(TestBehaviorMessages.EndMessage1);
        list[3].ShouldBe(TestBehaviorMessages.EndMessage3);
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

public class InheritWithBehaviorsAttribute : WithBehaviorsAttribute
{
    public InheritWithBehaviorsAttribute() :
        base(typeof(TestBehavior3<,>), typeof(TestBehavior1<,>))
    {
    }
}

[InheritWithBehaviors]
public class Req4 : IRequest<Res>
{
}

public class Req4Handler : IRequestHandler<Req4, Res>
{
    public Task<Res> Handle(Req4 request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Res());
    }
}
