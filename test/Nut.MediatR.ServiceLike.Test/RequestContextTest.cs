using FluentAssertions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class RequestContextTest
    {
        [Fact]
        public void ctor_pathがnullの場合は例外が発生する()
        {
            Action act = () => new RequestContext(null, typeof(ServicePing), typeof(Pong), new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_pathが空文字の場合は例外が発生する()
        {
            Action act = () => new RequestContext(string.Empty, typeof(ServicePing), typeof(Pong), new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_pathが空白文字の場合は例外が発生する()
        {
            Action act = () => new RequestContext(" ", typeof(ServicePing), typeof(Pong), new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_mediatorParameterTypeがnullの場合は例外が発生する()
        {
            Action act = () => new RequestContext("/this/is/path", null, typeof(Pong), new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_clientResultTypeがnullの場合は例外が発生する()
        {
            Action act = () => new RequestContext("/this/is/path", typeof(ServicePing), null, new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_serviceFactoryがnullの場合は例外が発生する()
        {
            Action act = () => new RequestContext("/this/is/path", typeof(ServicePing), typeof(Pong), null);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_コンストラクタで設定した値がプロパティで取得できる()
        {
            var path = "/this/is/path";
            var mediatorParameterType = typeof(ServicePing);
            var clientReusltType = typeof(Pong);
            var serviceFactory = new ServiceFactory(_ => null);

            var context = new RequestContext(path, mediatorParameterType, clientReusltType, serviceFactory);

            context.Path.Should().Be(path);
            context.MediatorParameterType.Should().Be(mediatorParameterType);
            context.ClientResultType.Should().Be(clientReusltType);
            context.ServiceFactory.Should().Be(serviceFactory);
        }
    }
}
