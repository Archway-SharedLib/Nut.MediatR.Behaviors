using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Add_AsServiceが複数付与されている場合は複数追加される()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(MultiServicePing));
            
            var req = registry.GetRequest("/ping/1");
            req.Should().NotBeNull();
            var req2 = registry.GetRequest("/ping/2");
            req2.Should().NotBeNull();

            registry.GetEndpoints().Should().HaveCount(2);
        }

        [Fact]
        public void Add_同じパスは例外が発生する()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            Action act = () => registry.Add(typeof(ServicePing2));

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

        [Fact]
        public void Add_設定したFilterがMediatorRequestに設定されている()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing), typeof(Filter1), typeof(Filter2));

            var request = registry.GetRequest("/ping");
            request.Filters.Should().HaveCount(2);
            var filters = request.Filters.ToList();
            filters[0].Should().Be(typeof(Filter1));
            filters[1].Should().Be(typeof(Filter2));
        }

        [Fact]
        public void Add_Filterを設定しないとMediatorRequestに設定されない()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            var request = registry.GetRequest("/ping");
            request.Filters.Should().HaveCount(0);
        }

        [Fact]
        public void Add_RequestにFilterが設定されている場合は末尾に追加される()
        {
            var registry = new RequestRegistry();
            registry.Add(typeof(ServiceWithFilterPing), typeof(Filter2), typeof(Filter3));

            var request = registry.GetRequest("/ping");
            request.Filters.Should().HaveCount(4);
            var filters = request.Filters.ToList();
            filters[0].Should().Be(typeof(Filter2));
            filters[1].Should().Be(typeof(Filter3));
            filters[2].Should().Be(typeof(Filter1));
            filters[3].Should().Be(typeof(Filter4));
        }

        [Fact]
        public void GetRequest_設定されていないパスが指定された場合はnullが返る()
        {
            var registry = new RequestRegistry();
            registry.GetRequest("/unknown/path").Should().BeNull();
        }
    }
}
