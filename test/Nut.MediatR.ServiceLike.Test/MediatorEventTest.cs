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
    public class MediatorEventTest
    {
        [Fact]
        public void Create_引数がnullの場合は例外が発行される()
        {
            Action act = () => MediatorEvent.Create(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_引数の型のGenericがオープンしてたら例外が発行される()
        {
            Action act = () => MediatorEvent.Create(typeof(OpenGenericPang<>));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型が実装型じゃない場合例外が発行される()
        {
            Action act = () => MediatorEvent.Create(typeof(AbstractPang));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型がINotificationを継承していない場合例外が発行される()
        {
            Action act = () => MediatorEvent.Create(typeof(PlainPang));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型にAsEventが付加されていない場合例外が発行される()
        {
            Action act = () => MediatorEvent.Create(typeof(OnlyPang));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_引数の型にINotificationを実装したクローズドでAsEventが付加されている場合はMediatorEventが返される()
        {
            var requests = MediatorEvent.Create(typeof(Pang));
            requests.Should().HaveCount(1);
            var request = requests.First();
            request.Key.Should().Be("pang");
            request.EventType.Should().Be(typeof(Pang));
        }

        [Fact]
        public void Create_引数の型に複数のAsEventが付加されている場合は複数のEventが返される()
        {
            var requests = MediatorEvent.Create(typeof(MultiPang));
            requests.Should().HaveCount(2);
            var requestList = requests.OrderBy(r => r.Key).ToList();
            requestList[0].Key.Should().Be("pang.1");
            requestList[0].EventType.Should().Be(typeof(MultiPang));
            requestList[1].Key.Should().Be("pang.2");
            requestList[1].EventType.Should().Be(typeof(MultiPang));
        }
    }
}
