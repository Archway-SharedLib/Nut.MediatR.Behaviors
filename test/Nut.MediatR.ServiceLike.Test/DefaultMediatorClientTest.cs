using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class DefaultMediatorClientTest
    {
        [Fact]
        public void ctor_mediatorがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(null, new RequestRegistry(), new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_requestRegistryがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(new Mediator(new ServiceFactory(_ => null))
                , null, new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_serviceFactoryがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(new Mediator(new ServiceFactory(_ => null))
                , new RequestRegistry(), null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SendAsync_requestがnullの場合は例外が発生する()
        {
            var serviceFactory = new ServiceFactory(_ => null);
            var client = new DefaultMediatorClient(new Mediator(serviceFactory)
                , new RequestRegistry(), serviceFactory);

            Func<Task> act = () => client.SendAsync<Pong>("/path", null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SendAsync_pathに一致するリクエストが見つからない場合は例外が発生する()
        {
            var serviceFactory = new ServiceFactory(_ => null);
            var client = new DefaultMediatorClient(new Mediator(serviceFactory)
                , new RequestRegistry(), serviceFactory);

            Func<Task> act = () => client.SendAsync<Pong>("/path", new ServicePing());
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task SendAsync_Mediatorが実行されて結果が返される()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();

            var serviceFactory = provider.GetService<ServiceFactory>();
            var mediator = provider.GetService<IMediator>();
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            var client = new DefaultMediatorClient(mediator, registry, serviceFactory);

            var pong = await client.SendAsync<Pong>("/ping", new ServicePing() { Value = "Ping" });
            pong.Value.Should().Be("Ping Pong");
        }

        [Fact]
        public async Task SendAsync_引数と戻り値は変換可能_Jsonシリアライズデシリアライズに依存()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();

            var serviceFactory = provider.GetService<ServiceFactory>();
            var mediator = provider.GetService<IMediator>();
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            var client = new DefaultMediatorClient(mediator, registry, serviceFactory);

            var pong = await client.SendAsync<LocalPong>("/ping", new { Value = "Ping" });
            pong.Value.Should().Be("Ping Pong");
        }

        [Fact]
        public async Task SendAsync_戻り値がnullの場合はnullが返される()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();

            var serviceFactory = provider.GetService<ServiceFactory>();
            var mediator = provider.GetService<IMediator>();
            var registry = new RequestRegistry();
            registry.Add(typeof(ServiceNullPing));

            var client = new DefaultMediatorClient(mediator, registry, serviceFactory);

            var pong = await client.SendAsync<Pong>("/ping/null", new { Value = "Ping" });
            pong.Should().BeNull();
        }

        [Fact]
        public async Task SendAsync_戻り値がUnitの場合はnullで返される()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(ServicePing).Assembly);
            var provider = services.BuildServiceProvider();

            var serviceFactory = provider.GetService<ServiceFactory>();
            var mediator = provider.GetService<IMediator>();
            var registry = new RequestRegistry();
            registry.Add(typeof(VoidServicePing));

            var client = new DefaultMediatorClient(mediator, registry, serviceFactory);

            var pong = await client.SendAsync<Pong>("/ping/void", new { Value = "Ping" });
            pong.Should().BeNull();
        }


        public class LocalPong
        {
            public string Value { get; set; }
        }
    }
}
