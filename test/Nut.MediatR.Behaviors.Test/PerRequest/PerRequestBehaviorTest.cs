using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
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
        var list = new List<string>();
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(Arg.Any<Type>()).Returns(ci =>
        {
            return Activator.CreateInstance(ci.Arg<Type>(), list);
        });
        var perReq = new PerRequestBehavior<Req1, Res>(provider);
        await perReq.Handle(new Req1(), () => Task.FromResult(new Res()), new CancellationToken());

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
        var list = new List<string>();
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(Arg.Any<Type>()).Returns(ci =>
        {
            return Activator.CreateInstance(ci.Arg<Type>(), list);
        });
        var perReq = new PerRequestBehavior<Req2, Res>(provider);
        await perReq.Handle(new Req2(), () => Task.FromResult(new Res()), new CancellationToken());
        list.Count.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ServiceFactoryからインスタンスが返されないBehaviorはスキップされる()
    {
        var list = new List<string>();
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(Arg.Any<Type>()).Returns(ci =>
        {
            var type = ci.Arg<Type>();
            if (type == typeof(TestBehavior1<Req1, Res>)) return null;
            return Activator.CreateInstance(type, list);
        });
        //var factory = new ServiceFactory(type =>
        //{
        //    if (type == typeof(TestBehavior1<Req1, Res>)) return null;
        //    return Activator.CreateInstance(type, list);
        //});
        var perReq = new PerRequestBehavior<Req1, Res>(provider);
        await perReq.Handle(new Req1(), () => Task.FromResult(new Res()), new CancellationToken());

        list.Count.Should().Be(4);
        list[0].Should().Be(TestBehaviorMessages.StartMessage3);
        list[1].Should().Be(TestBehaviorMessages.StartMessage2);
        list[2].Should().Be(TestBehaviorMessages.EndMessage2);
        list[3].Should().Be(TestBehaviorMessages.EndMessage3);
    }
}

public class Res
{
}

[WithBehaviors(typeof(TestBehavior3<,>), typeof(TestBehavior1<,>), typeof(TestBehavior2<,>))]
public class Req1 : IRequest<Res>
{
}

public class Req2 : IRequest<Res>
{
}
