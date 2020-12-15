using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class RequestRegistryTest
    {
        [Fact]
        public void Add_AsServiceが付与されている場合は追加される()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));
            var req = registry.GetRequest("/ping");
            req.Should().NotBeNull();
            registry.GetEndpoints().Should().HaveCount(1);
        }

        [Fact]
        public void Add_AsServiceが複数付与されている場合は複数追加されされない()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(MultiServicePing));
            
            var req = registry.GetRequest("/ping/1");
            req.Should().NotBeNull();
            var req2 = registry.GetRequest("/ping/2");
            req.Should().NotBeNull();

            registry.GetEndpoints().Should().HaveCount(2);
        }

        [Fact]
        public void Add_同じパスは例外が発生する()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            Action act = () => registry.Add(typeof(ServicePing));

            act.Should().Throw<ArgumentException>();
            registry.GetEndpoints().Should().HaveCount(1);
        }

        [Fact]
        public void Add_ignoreDuplicationをfalseにすると同じパスは例外が発生する()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            Action act = () => registry.Add(typeof(ServicePing2), false);

            act.Should().Throw<ArgumentException>();
            registry.GetEndpoints().Should().HaveCount(1);
        }

        [Fact]
        public void Add_ignoreDuplicationをtrueにすると同じパスは無視される()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            registry.Add(typeof(ServicePing2), true);

            registry.GetEndpoints().Should().HaveCount(1);
            var type = registry.GetRequest("/ping").RequestType;

            type.Should().Be(typeof(ServicePing));
        }

        [Fact]
        public void Add_typeがnullの場合は例外が発生する()
        {
            var registry = new RequestRegistry();
            Action act = () => registry.Add(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Add_typeがnullの場合は例外が発生する_withIgnoreDuplication()
        {
            var registry = new RequestRegistry();
            Action act = () => registry.Add(null, true);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
