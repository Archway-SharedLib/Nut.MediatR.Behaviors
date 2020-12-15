using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nut.MediatR.ServiceLike.DependencyInjection.Test
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddMediatRServiceLike_アセンブリを探索してAsServiceがついたIRequestが自動的に登録される()
        {
            var services = new ServiceCollection();
            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();

            var registry = provider.GetService<RequestRegistry>();
            registry.GetRequest("/ping").Should().NotBeNull();
            registry.GetRequest("/ping/void").Should().NotBeNull();
        }

        [Fact]
        public void AddMediatRServiceLike_RequerstRegistryが先に登録されている場合はそのインスタンスが利用される()
        {
            var services = new ServiceCollection();
            var registry = new RequestRegistry();
            services.AddSingleton(registry);

            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();
            var registryFromService = provider.GetService<RequestRegistry>();
            
            registryFromService.Should().BeSameAs(registry);
            registry.GetRequest("/ping").Should().NotBeNull();
            registry.GetRequest("/ping/void").Should().NotBeNull();
        }
    }
}
