using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test;

public class RequestContextTest
{
    [Fact]
    public void ctor_pathがnullの場合は例外が発生する()
    {
        Action act = () => new RequestContext(null, typeof(ServicePing), new ServiceFactory(_ => null), typeof(Pong));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ctor_pathが空文字の場合は例外が発生する()
    {
        Action act = () => new RequestContext(string.Empty, typeof(ServicePing), new ServiceFactory(_ => null), typeof(Pong));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ctor_pathが空白文字の場合は例外が発生する()
    {
        Action act = () => new RequestContext(" ", typeof(ServicePing), new ServiceFactory(_ => null), typeof(Pong));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ctor_mediatorParameterTypeがnullの場合は例外が発生する()
    {
        Action act = () => new RequestContext("/this/is/path", null, new ServiceFactory(_ => null), typeof(Pong));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ctor_serviceFactoryがnullの場合は例外が発生する()
    {
        Action act = () => new RequestContext("/this/is/path", typeof(ServicePing), null, typeof(Pong));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ctor_コンストラクタで設定した値がプロパティで取得できる()
    {
        var path = "/this/is/path";
        var mediatorParameterType = typeof(ServicePing);
        var clientResultType = typeof(Pong);
        var serviceFactory = new ServiceFactory(_ => null);

        var context = new RequestContext(path, mediatorParameterType, serviceFactory, clientResultType);

        context.Path.Should().Be(path);
        context.MediatorParameterType.Should().Be(mediatorParameterType);
        context.ClientResultType.Should().Be(clientResultType);
        context.ServiceFactory.Should().Be(serviceFactory);
    }

    [Fact]
    public void NeedClientResult_ClientResultTypeがnullの場合はfalseになる()
    {
        var path = "/this/is/path";
        var mediatorParameterType = typeof(ServicePing);
        var serviceFactory = new ServiceFactory(_ => null);

        var context = new RequestContext(path, mediatorParameterType, serviceFactory);
        context.ClientResultType.Should().BeNull();
        context.NeedClientResult.Should().BeFalse();
    }

    [Fact]
    public void NeedClientResult_ClientResultTypeがある場合はtrueになる()
    {
        var path = "/this/is/path";
        var mediatorParameterType = typeof(ServicePing);
        var serviceFactory = new ServiceFactory(_ => null);
        var clientResultType = typeof(Pong);

        var context = new RequestContext(path, mediatorParameterType, serviceFactory, clientResultType);
        context.ClientResultType.Should().Be(clientResultType);
        context.NeedClientResult.Should().BeTrue();
    }
}
