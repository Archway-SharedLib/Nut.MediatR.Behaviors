using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class DefaultMediatorClientTest
    {
        //suppress obsolete warning
#pragma warning disable CS0618
        [Fact]
        public void obsolete_ctor_mediatorがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(null, new RequestRegistry(), new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void obsolete_ctor_requestRegistryがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(new Mediator(new ServiceFactory(_ => null))
                , null, new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void obsolete_ctor_serviceFactoryがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(new Mediator(new ServiceFactory(_ => null))
                , new RequestRegistry(), null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void obsolete_ctor_インスタンスの生成()
        {
            Action act = () => new DefaultMediatorClient(new Mediator(new ServiceFactory(_ => null))
                , new RequestRegistry(), new ServiceFactory(_ => null));
            act.Should().NotThrow<ArgumentNullException>();
        }
#pragma warning restore CS0618

        [Fact]
        public void ctor_requestRegistryがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(null, new NotificationRegistry(), new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_notificationRegistryがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(new RequestRegistry(), null, new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_serviceFactoryがnullの場合は例外が発生する()
        {
            Action act = () => new DefaultMediatorClient(new RequestRegistry(), new NotificationRegistry(), null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SendAsync_requestがnullの場合は例外が発生する()
        {
            var serviceFactory = new ServiceFactory(_ => null);
            var client = new DefaultMediatorClient(new RequestRegistry(), new NotificationRegistry(), serviceFactory);

            Func<Task> act = () => client.SendAsync<Pong>("/path", null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SendAsync_pathに一致するリクエストが見つからない場合は例外が発生する()
        {
            var serviceFactory = new ServiceFactory(_ => null);
            var client = new DefaultMediatorClient(new RequestRegistry(), new NotificationRegistry(), serviceFactory);

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
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            var client = new DefaultMediatorClient(registry, new NotificationRegistry(), serviceFactory);

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
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing));

            var client = new DefaultMediatorClient(registry, new NotificationRegistry(), serviceFactory);

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
            var registry = new RequestRegistry();
            registry.Add(typeof(ServiceNullPing));

            var client = new DefaultMediatorClient(registry, new NotificationRegistry(), serviceFactory);

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
            var registry = new RequestRegistry();
            registry.Add(typeof(VoidServicePing));

            var client = new DefaultMediatorClient(registry, new NotificationRegistry(), serviceFactory);

            var pong = await client.SendAsync<Pong>("/ping/void", new { Value = "Ping" });
            pong.Should().BeNull();
        }

        [Fact]
        public async Task SendAsync_Filterが設定されている場合は順番に実行される()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(ServicePing).Assembly);

            var check = new FilterExecutionCheck();
            services.AddSingleton(check);
            var provider = services.BuildServiceProvider();

            var serviceFactory = provider.GetService<ServiceFactory>();
            var registry = new RequestRegistry();
            registry.Add(typeof(ServicePing), typeof(Filter1), typeof(Filter2));

            var client = new DefaultMediatorClient(registry, new NotificationRegistry(), serviceFactory);

            var pong = await client.SendAsync<Pong>("/ping", new ServicePing() { Value = "Ping" });

            pong.Value.Should().Be("Ping Pong");
            check.Checks.Should().HaveCount(2);
            check.Checks[0].Should().Be("1");
            check.Checks[1].Should().Be("2");
        }

        [Fact]
        public void PublisAsync_requestがnullの場合は例外が発生する()
        {
            var serviceFactory = new ServiceFactory(_ => null);
            var client = new DefaultMediatorClient(new RequestRegistry(), new NotificationRegistry(), serviceFactory);

            Func<Task> act = () => client.PublishAsync("ev", null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PublisAsync_keyに一致するイベントが見つからない場合は例外が発生する()
        {
            var serviceFactory = new ServiceFactory(_ => null);
            var client = new DefaultMediatorClient(new RequestRegistry(), new NotificationRegistry(), serviceFactory);

            Func<Task> act = () => client.PublishAsync("key", new Pang());
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task PublishAsync_Mediatorが実行される()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(ServicePing).Assembly);

            services.AddSingleton<TaskHolder>();
            var provider = services.BuildServiceProvider();

            var serviceFactory = provider.GetService<ServiceFactory>();
            var registry = new NotificationRegistry();
            registry.Add(typeof(MediatorClientTestPang));

            var client = new DefaultMediatorClient(new RequestRegistry(), registry, serviceFactory);
            var holder = provider.GetService<TaskHolder>();

            var pang = new MediatorClientTestPang();
            await client.PublishAsync(nameof(MediatorClientTestPang), pang);

            await Task.WhenAll(holder.Tasks);
            holder.Messages.Should().HaveCount(3).And.Contain("1", "2", "3");
            holder.Pangs.Should().HaveCount(3);

            var paramBang = holder.Pangs[0];
            foreach (var bangItem in holder.Pangs)
            {
                paramBang.Should().Be(bangItem);
            }
        }


        public class LocalPong
        {
            public string Value { get; set; }
        }

        [AsEvent(nameof(MediatorClientTestPang))]
        public class MediatorClientTestPang: INotification
        {
        }

        public class TaskHolder
        {
            public List<Task> Tasks { get; } = new();

            public List<string> Messages { get; } = new();

            public List<MediatorClientTestPang> Pangs { get; } = new(); 
        }

        public class MediatorClientTestHandlerBase
        {
            private readonly TaskHolder holder;
            private readonly Task task;

            protected MediatorClientTestHandlerBase(TaskHolder holder, string message)
            {
                this.task = new Task(() =>
                {
                    Thread.Sleep(10);
                    holder.Messages.Add(message);
                });
                this.holder = holder;
                holder.Tasks.Add(this.task);
            }
            public Task Handle(MediatorClientTestPang notification, CancellationToken cancellationToken)
            {
                holder.Pangs.Add(notification);
                this.task.Start();
                return Task.CompletedTask;
            }
        }

        public class MediatorClientTestHandler1 : MediatorClientTestHandlerBase,  INotificationHandler<MediatorClientTestPang>
        {
            public MediatorClientTestHandler1(TaskHolder holder) : base(holder, "1")
            {
            }
        }
        public class MediatorClientTestHandler2 : MediatorClientTestHandlerBase, INotificationHandler<MediatorClientTestPang>
        {
            public MediatorClientTestHandler2(TaskHolder holder) : base(holder, "2")
            {
            }
        }

        public class MediatorClientTestHandler3 : MediatorClientTestHandlerBase, INotificationHandler<MediatorClientTestPang>
        {
            public MediatorClientTestHandler3(TaskHolder holder) : base(holder, "3")
            {
            }
        }


    }
}
