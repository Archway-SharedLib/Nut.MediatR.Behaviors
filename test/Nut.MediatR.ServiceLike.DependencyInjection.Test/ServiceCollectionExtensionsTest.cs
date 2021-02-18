using FluentAssertions;
using MediatR;
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

            var registry = provider.GetService<ServiceRegistry>();
            registry.GetService("/ping").Should().NotBeNull();
            registry.GetService("/ping/void").Should().NotBeNull();
        }

        [Fact]
        public void AddMediatRServiceLike_アセンブリを探索してAsEventがついたINotificationが自動的に登録される()
        {
            var services = new ServiceCollection();
            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();

            var registry = provider.GetService<ListenerRegistry>();
            registry.GetListeners("pang").Should().NotBeNull();
            registry.GetListeners("pang2").Should().NotBeNull();
        }

        [Fact]
        public void AddMediatRServiceLike_RequerstRegistryが先に登録されている場合はそのインスタンスが利用される()
        {
            var services = new ServiceCollection();
            var registry = new ServiceRegistry();
            services.AddSingleton(registry);

            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();
            var registryFromService = provider.GetService<ServiceRegistry>();
            
            registryFromService.Should().BeSameAs(registry);
            registry.GetService("/ping").Should().NotBeNull();
            registry.GetService("/ping/void").Should().NotBeNull();
        }

        [Fact]
        public void AddMediatRServiceLike_EventRegistryが先に登録されている場合はそのインスタンスが利用される()
        {
            var services = new ServiceCollection();
            var registry = new ListenerRegistry();
            services.AddSingleton(registry);

            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();
            var registryFromService = provider.GetService<ListenerRegistry>();

            registryFromService.Should().BeSameAs(registry);
            registry.GetListeners("pang").Should().NotBeNull();
            registry.GetListeners("pang2").Should().NotBeNull();
        }

        [Fact]
        public void AddMediatRServiceLike_IMediatorClientはDefaultMediatorClientでIMediatorが無い方のコンストラクタが利用される()
        {
            var services = new ServiceCollection();
            var serviceRegistry = new ServiceRegistry();
            services.AddSingleton(serviceRegistry);
            var listenerRegistry = new ListenerRegistry();
            services.AddSingleton(listenerRegistry);
            var serviceFactory = new ServiceFactory(_ => null);
            services.AddSingleton(serviceFactory);

            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);

            var client = services.BuildServiceProvider().GetService<IMediatorClient>();
            client.Should().NotBeNull().And.BeOfType<DefaultMediatorClient>();
        }
    }
}
