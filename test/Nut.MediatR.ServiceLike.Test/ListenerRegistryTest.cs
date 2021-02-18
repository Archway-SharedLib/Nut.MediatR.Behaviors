using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class ListenerRegistryTest
    {
        [Fact]
        public void Add_AsEventが付与されている場合は追加される()
        {
            var registry = new ListenerRegistry();
            registry.Add(typeof(Pang));
            var ev = registry.GetListeners("pang");
            ev.Should().NotBeNull();
            registry.GetKeys().Should().HaveCount(1);
        }

        [Fact]
        public void Add_AsEventが複数付与されている場合は複数追加される()
        {
            var registry = new ListenerRegistry();
            registry.Add(typeof(MultiPang));
            
            var ev1 = registry.GetListeners("pang.1");
            ev1.Should().NotBeNull();
            var ev2 = registry.GetListeners("pang.2");
            ev2.Should().NotBeNull();

            registry.GetKeys().Should().HaveCount(2);
        }

        [Fact]
        public void Add_同じパスが登録された場合は両方とも保持される()
        {
            var registry = new ListenerRegistry();
            registry.Add(typeof(Pang));
            registry.Add(typeof(Pang2));

            registry.GetKeys().Should().HaveCount(1);
            var notifications = registry.GetListeners(registry.GetKeys().First());
            notifications.Should().HaveCount(2);
            var expectedList = new Queue<Type>(new [] { typeof(Pang), typeof(Pang2) });
            for(var i = 0; i < expectedList.Count; i++)
            {
                var expect = expectedList.Dequeue();
                notifications.Should().Contain(n => n.ListenerType == expect);
            }
        }

        [Fact]
        public void Add_typeがnullの場合は例外が発生する()
        {
            var registry = new ListenerRegistry();
            Action act = () => registry.Add(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetNotifications_設定されていないパスが指定された場合はnullが返る()
        {
            var registry = new ListenerRegistry();
            registry.GetListeners("unknown.event").Should().BeEmpty();
        }
    }
}
