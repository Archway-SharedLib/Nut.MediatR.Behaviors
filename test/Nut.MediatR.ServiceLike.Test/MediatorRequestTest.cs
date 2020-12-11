using FluentAssertions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class MediatorRequestTest
    {
        [Fact]
        public void Create_引数がnullの場合は例外が発行される()
        {
            Action act = () => MediatorRequest.Create(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_引数の型のGenericがオープンしてたら例外が発行される()
        {
            Action act = () => MediatorRequest.Create(typeof(TestOpenGenericRequest<>));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型が実装型じゃない場合例外が発行される()
        {
            Action act = () => MediatorRequest.Create(typeof(TestAbstractRequest));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型がIRequestを継承していない場合例外が発行される()
        {
            Action act = () => MediatorRequest.Create(typeof(TestPlanRequest));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型にAsServiceが付加されていない場合例外が発行される()
        {
            Action act = () => MediatorRequest.Create(typeof(TestNotServicRequest));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型にIRequestを実装したクローズドでAsServiceが付加されている場合はMediatorRequestが返される()
        {
            var requests = MediatorRequest.Create(typeof(TestServiceRequest));
            requests.Should().HaveCount(1);
            var request = requests.First();
            request.Path.Should().Be("/path");
            request.RequestType.Should().Be(typeof(TestServiceRequest));
        }

        [Fact]
        public void Create_引数の型にIRequestTを実装したクローズドでAsServiceが付加されている場合はMediatorRequestが返される()
        {
            var requests = MediatorRequest.Create(typeof(TestServiceRequestT));
            requests.Should().HaveCount(1);
            var request = requests.First();
            request.Path.Should().Be("/path");
            request.RequestType.Should().Be(typeof(TestServiceRequestT));
        }

        [Fact]
        public void Create_引数の型に複数のAsServiceが付加されている場合は複数のRequestが返される()
        {
            var requests = MediatorRequest.Create(typeof(TestMultipleRouteRequest));
            requests.Should().HaveCount(2);
            var requestList = requests.OrderBy(r => r.Path).ToList();
            requestList[0].Path.Should().Be("/path1");
            requestList[0].RequestType.Should().Be(typeof(TestMultipleRouteRequest));
            requestList[1].Path.Should().Be("/path2");
            requestList[1].RequestType.Should().Be(typeof(TestMultipleRouteRequest));
        }

        [AsService("/path")]
        public class TestOpenGenericRequest<T> : IRequest<T> { }

        [AsService("/path")]
        public abstract class TestAbstractRequest : IRequest<Unit> { }

        [AsService("/path")]
        public class TestPlanRequest { }

        public abstract class TestNotServicRequest : IRequest<Unit> { }

        [AsService("/path")]
        public class TestServiceRequest: IRequest { }

        [AsService("/path")]
        public class TestServiceRequestT : IRequest<Unit> { }

        [AsService("/path1")]
        [AsService("/path2")]
        public class TestMultipleRouteRequest : IRequest<Unit> { }
    }
}
