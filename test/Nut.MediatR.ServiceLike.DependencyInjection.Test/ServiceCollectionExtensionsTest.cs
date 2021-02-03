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

            var registry = provider.GetService<RequestRegistry>();
            registry.GetRequest("/ping").Should().NotBeNull();
            registry.GetRequest("/ping/void").Should().NotBeNull();
        }

        [Fact]
        public void AddMediatRServiceLike_アセンブリを探索してAsEventがついたINotificationが自動的に登録される()
        {
            var services = new ServiceCollection();
            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();

            var registry = provider.GetService<NotificationRegistry>();
            registry.GetNotifications("pang").Should().NotBeNull();
            registry.GetNotifications("pang2").Should().NotBeNull();
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

        [Fact]
        public void AddMediatRServiceLike_EventRegistryが先に登録されている場合はそのインスタンスが利用される()
        {
            var services = new ServiceCollection();
            var registry = new NotificationRegistry();
            services.AddSingleton(registry);

            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();
            var registryFromService = provider.GetService<NotificationRegistry>();

            registryFromService.Should().BeSameAs(registry);
            registry.GetNotifications("pang").Should().NotBeNull();
            registry.GetNotifications("pang2").Should().NotBeNull();
        }

        [Fact]
        public void AddMediatRServiceLike_IMediatorClientはDefaultMediatorClientでIMediatorが無い方のコンストラクタが利用される()
        {
            var services = new ServiceCollection();
            var requestRegistry = new RequestRegistry();
            services.AddSingleton(requestRegistry);
            var notificationRegistry = new NotificationRegistry();
            services.AddSingleton(notificationRegistry);
            var serviceFactory = new ServiceFactory(_ => null);
            services.AddSingleton(serviceFactory);

            services.AddMediatRServiceLike(typeof(ServicePing).Assembly);

            var client = services.BuildServiceProvider().GetService<IMediatorClient>();
            client.Should().NotBeNull().And.BeOfType<DefaultMediatorClient>();
        }
    }
}
