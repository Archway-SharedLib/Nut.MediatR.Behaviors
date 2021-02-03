using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class EventRegistryTest
    {
        [Fact]
        public void Add_AsEventが付与されている場合は追加される()
        {
            var registry = new EventRegistry();
            registry.Add(typeof(Pang));
            var ev = registry.GetEvent("pang");
            ev.Should().NotBeNull();
            registry.GetKeys().Should().HaveCount(1);
        }

        [Fact]
        public void Add_AsEventが複数付与されている場合は複数追加される()
        {
            var registry = new EventRegistry();
            registry.Add(typeof(MultiPang));
            
            var ev1 = registry.GetEvent("pang.1");
            ev1.Should().NotBeNull();
            var ev2 = registry.GetEvent("pang.2");
            ev2.Should().NotBeNull();

            registry.GetKeys().Should().HaveCount(2);
        }

        [Fact]
        public void Add_同じパスは例外が発生する()
        {
            var registry = new EventRegistry();
            registry.Add(typeof(Pang));

            Action act = () => registry.Add(typeof(Pang2));

            act.Should().Throw<ArgumentException>();
            registry.GetKeys().Should().HaveCount(1);
        }

        [Fact]
        public void Add_ignoreDuplicationをfalseにすると同じパスは例外が発生する()
        {
            var registry = new EventRegistry();
            registry.Add(typeof(Pang));

            Action act = () => registry.Add(typeof(Pang2), false);

            act.Should().Throw<ArgumentException>();
            registry.GetKeys().Should().HaveCount(1);
        }

        [Fact]
        public void Add_ignoreDuplicationをtrueにすると同じパスは無視される()
        {
            var registry = new EventRegistry();
            registry.Add(typeof(Pang));

            registry.Add(typeof(Pang2), true);

            registry.GetKeys().Should().HaveCount(1);
            var type = registry.GetEvent("pang").EventType;

            type.Should().Be(typeof(Pang));
        }

        [Fact]
        public void Add_typeがnullの場合は例外が発生する()
        {
            var registry = new EventRegistry();
            Action act = () => registry.Add(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Add_typeがnullの場合は例外が発生する_withIgnoreDuplication()
        {
            var registry = new EventRegistry();
            Action act = () => registry.Add(null, true);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetEvent_設定されていないパスが指定された場合はnullが返る()
        {
            var registry = new EventRegistry();
            registry.GetEvent("unknown.event").Should().BeNull();
        }
    }
}
