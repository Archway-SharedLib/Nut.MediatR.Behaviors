using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test;

public class DefaultMediatorClientTest
{

    [Fact]
    public void ctor_requestRegistryがnullの場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        Action act = () => new DefaultMediatorClient(null, new ListenerRegistry(),
            serviceFactory, new InternalScopedServiceFactoryFactory(serviceFactory), new TestLogger());
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ctor_notificationRegistryがnullの場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        Action act = () => new DefaultMediatorClient(new ServiceRegistry(), null!,
            serviceFactory, new InternalScopedServiceFactoryFactory(serviceFactory), new TestLogger());
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ctor_serviceFactoryがnullの場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        Action act = () => new DefaultMediatorClient(new ServiceRegistry(), new ListenerRegistry(),
            null!, new InternalScopedServiceFactoryFactory(serviceFactory), new TestLogger());
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ctor_ScopedServiceFactoryFactoryがnullの場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        Action act = () => new DefaultMediatorClient(new ServiceRegistry(), new ListenerRegistry(),
            serviceFactory, null!, new TestLogger());
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task T_SendAsync_requestがnullの場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        var client = new DefaultMediatorClient(new ServiceRegistry(), new ListenerRegistry(),
            serviceFactory, new InternalScopedServiceFactoryFactory(serviceFactory), new TestLogger());

        Func<Task> act = () => client.SendAsync<Pong>("/path", null);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task T_SendAsync_pathに一致するリクエストが見つからない場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        var client = new DefaultMediatorClient(new ServiceRegistry(), new ListenerRegistry(),
            serviceFactory, new InternalScopedServiceFactoryFactory(serviceFactory), new TestLogger());

        var act = () => client.SendAsync<Pong>("/path", new ServicePing());
        var res = await act.Should().ThrowAsync<ReceiverNotFoundException>();
        res.And.RequestPath.Should().Be("/path");
    }

    [Fact]
    public async Task T_SendAsync_Mediatorが実行されて結果が返される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>()!;
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServicePing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var pong = await client.SendAsync<Pong>("/ping", new ServicePing() { Value = "Ping" });
        pong!.Value.Should().Be("Ping Pong");
        check.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_Mediatorが実行されるが結果は捨てられる()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>()!;
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServicePing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        await client.SendAsync("/ping", new ServicePing() { Value = "Ping" });
        check.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task T_SendAsync_引数と戻り値は変換可能_Jsonシリアライズデシリアライズに依存()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServicePing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var pong = await client.SendAsync<LocalPong>("/ping", new { Value = "Ping" });
        pong!.Value.Should().Be("Ping Pong");
        check.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task T_SendAsync_引数を変換できない場合は例外が発生する()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServicePing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var act = () => client.SendAsync<Pong>("/ping", "ping");
        var res = await act.Should().ThrowAsync<TypeTranslationException>();
        res.And.FromType.Should().Be(typeof(string));
        res.And.ToType.Should().Be(typeof(ServicePing));
    }

    [Fact]
    public async Task T_SendAsync_戻り値を変換できない場合は例外が発生する()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServicePing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var act = () => client.SendAsync<string>("/ping", new { Value = "Ping" });
        var res = await act.Should().ThrowAsync<TypeTranslationException>();
        res.And.FromType.Should().Be(typeof(Pong));
        res.And.ToType.Should().Be(typeof(string));
    }

    [Fact]
    public async Task T_SendAsync_戻り値がnullの場合はnullが返される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServiceNullPing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var pong = await client.SendAsync<Pong>("/ping/null", new { Value = "Ping" });
        pong.Should().BeNull();
        check.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_戻り値がnullの場合も結果が捨てられる()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServiceNullPing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        await client.SendAsync("/ping/null", new { Value = "Ping" });
        check.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task T_SendAsync_戻り値がUnitの場合はnullで返される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(VoidServicePing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var pong = await client.SendAsync<Pong>("/ping/void", new { Value = "Ping" });
        pong.Should().BeNull();
        check.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_戻り値がUnitの場合も結果は捨てられる()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);
        var check = new ExecuteCheck();
        services.AddTransient(_ => check);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(VoidServicePing));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        await client.SendAsync("/ping/void", new { Value = "Ping" });
        check.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task T_SendAsync_Filterが設定されている場合は順番に実行される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);

        var check = new FilterExecutionCheck();
        services.AddSingleton(check);

        var handlerCheck = new ExecuteCheck();
        services.AddTransient(_ => handlerCheck);

        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ServiceRegistry();
        registry.Add(typeof(ServicePing), typeof(Filter1), typeof(Filter2));

        var client = new DefaultMediatorClient(registry, new ListenerRegistry(),
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var pong = await client.SendAsync<Pong>("/ping", new ServicePing() { Value = "Ping" });

        pong!.Value.Should().Be("Ping Pong");
        check.Checks.Should().HaveCount(2);
        check.Checks[0].Should().Be("1");
        check.Checks[1].Should().Be("2");
    }

    [Fact]
    public async Task PublishAsyncActionOption_actionがnullの場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        var client = new DefaultMediatorClient(new ServiceRegistry(), new ListenerRegistry(),
            serviceFactory, new InternalScopedServiceFactoryFactory(serviceFactory!), new TestLogger());

        var act = () => client.PublishAsync("ev", new Pang(), (Action<PublishOptions>)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishAsync_requestがnullの場合は例外が発生する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        var client = new DefaultMediatorClient(new ServiceRegistry(), new ListenerRegistry(),
            serviceFactory, new InternalScopedServiceFactoryFactory(serviceFactory!), new TestLogger());

        Func<Task> act = () => client.PublishAsync("ev", null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void PublishAsync_keyに一致するイベントが見つからない場合はなにも実行されず終了する()
    {
        var serviceFactory = new ServiceFactory(_ => null);
        var client = new DefaultMediatorClient(new ServiceRegistry(), new ListenerRegistry(),
            serviceFactory, new InternalScopedServiceFactoryFactory(serviceFactory!), new TestLogger());

        client.PublishAsync("key", new Pang());

        //TODO: assertion
    }

    [Fact]
    public async Task PublishAsyncActionOption_Mediatorが実行される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);

        services.AddSingleton<TaskHolder>();
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ListenerRegistry();
        registry.Add(typeof(MediatorClientTestPang));

        var client = new DefaultMediatorClient(new ServiceRegistry(), registry,
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var holder = provider.GetService<TaskHolder>();

        var pang = new MediatorClientTestPang();
        await client.PublishAsync(nameof(MediatorClientTestPang), pang, optionsAction: op => {});

        await Task.Delay(1000); //それぞれで10だけまたしているため、1000あれば終わっているはず。

        await Task.WhenAll(holder.Tasks);
        holder.Messages.Should().HaveCount(3).And.Contain("1", "2", "3");
        holder.Pangs.Should().HaveCount(3);

        var paramBang = holder.Pangs[0];
        foreach (var bangItem in holder.Pangs)
        {
            paramBang.Should().Be(bangItem);
        }
    }

    [Fact]
    public async Task PublishAsync_Mediatorが実行される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ServicePing).Assembly);

        services.AddSingleton<TaskHolder>();
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ListenerRegistry();
        registry.Add(typeof(MediatorClientTestPang));

        var client = new DefaultMediatorClient(new ServiceRegistry(), registry,
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        var holder = provider.GetService<TaskHolder>();

        var pang = new MediatorClientTestPang();
        await client.PublishAsync(nameof(MediatorClientTestPang), pang);

        Thread.Sleep(1000); //それぞれで10だけまたしているため、1000あれば終わっているはず。

        await Task.WhenAll(holder.Tasks);
        holder.Messages.Should().HaveCount(3).And.Contain("1", "2", "3");
        holder.Pangs.Should().HaveCount(3);

        var paramBang = holder.Pangs[0];
        foreach (var bangItem in holder.Pangs)
        {
            paramBang.Should().Be(bangItem);
        }
    }

    [Fact]
    public async Task PublishAsync_Notification内で例外が発生しても続行される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(ExceptionPang).Assembly);
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ListenerRegistry();
        registry.Add(typeof(ExceptionPang));

        var logger = new TestLogger();

        var client = new DefaultMediatorClient(new ServiceRegistry(), registry,
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), logger);
        await client.PublishAsync(nameof(ExceptionPang), new { });

        // Fire and forgetのため一旦スリープ
        Thread.Sleep(2000);

        logger.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task PublishAsync_RequestもNotificationも実行される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(MixedRequest).Assembly);
        services.AddSingleton<MixedTaskHolder>();
        services.AddScoped<ScopeIdProvider>();
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ListenerRegistry();
        registry.Add(typeof(MixedRequest));
        registry.Add(typeof(MixedNotification));

        var holder = provider.GetService<MixedTaskHolder>();

        var client = new DefaultMediatorClient(new ServiceRegistry(), registry,
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        await client.PublishAsync("mixed", new { });

        // Fire and forgetのため一旦スリープ
        Thread.Sleep(1000);

        holder.Messages.Should().HaveCount(3);
        holder.Messages.Contains("request").Should().BeTrue();
        holder.Messages.Contains("notification").Should().BeTrue();
        holder.Messages.Contains("notification2").Should().BeTrue();
    }

    [Fact]
    public async Task PublishAsync_Listener実行前に例外が発生した場合はリスナーが実行されずにログが出力される()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(MixedRequest).Assembly);
        services.AddSingleton<MixedTaskHolder>();
        services.AddScoped<ScopeIdProvider>();
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ListenerRegistry();
        registry.Add(typeof(MixedRequest));
        registry.Add(typeof(MixedNotification));

        var holder = provider.GetService<MixedTaskHolder>();

        var testLogger = new TestLogger();
        var client = new DefaultMediatorClient(new ServiceRegistry(), registry,
            serviceFactory!, new ExceptionTestScopedServiceFactoryFactory(), testLogger);

        await client.PublishAsync("mixed", new { });

        // Fire and forgetのため一旦スリープ
        Thread.Sleep(1000);

        holder.Messages.Should().HaveCount(0);
        testLogger.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task PublishAsync_別のScopeで実行されるが同じINotificationは同じScope()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(MixedRequest).Assembly);
        services.AddSingleton<MixedTaskHolder>();
        services.AddScoped<ScopeIdProvider>();
        var provider = services.BuildServiceProvider();

        var serviceFactory = provider.GetService<ServiceFactory>();
        var registry = new ListenerRegistry();
        registry.Add(typeof(MixedRequest));
        registry.Add(typeof(MixedNotification));

        var holder = provider.GetService<MixedTaskHolder>();

        var client = new DefaultMediatorClient(new ServiceRegistry(), registry,
            serviceFactory!, new TestScopedServiceFactoryFactory(provider), new TestLogger());

        await client.PublishAsync("mixed", new { });

        // Fire and forgetのため一旦スリープ
        await Task.Delay(1000);

        holder.ScopeIds.Should().HaveCount(3);
        holder.ScopeIds[typeof(MixedRequestHandler)].Should().NotBe(holder.ScopeIds[typeof(MixedNotificationHandler)]);
        holder.ScopeIds[typeof(MixedNotificationHandler)].Should().Be(holder.ScopeIds[typeof(MixedNotificationHandler2)]);

        holder.ScopeIdProviders.All(p => p.Disposed).Should().BeTrue();
    }

    private class ExceptionTestScopedServiceFactoryFactory : IScopedServiceFactoryFactory
    {
        public IScoepedServiceFactory Create()
        {
            throw new NotImplementedException();
        }
    }

    [AsEventListener("mixed")]
    public record MixedRequest : IRequest;

    [AsEventListener("mixed")]
    public record MixedNotification : INotification;

    public class MixedTaskHolder
    {
        public List<string> Messages { get; } = new();

        public Dictionary<Type, string> ScopeIds { get; } = new();

        public List<ScopeIdProvider> ScopeIdProviders { get; } = new();
    }

    public class ScopeIdProvider: IDisposable
    {
        public ScopeIdProvider()
        {
            _value = Guid.NewGuid().ToString();
        }

        private string _value;
        public string Value
        {
            get
            {
                return Disposed ? throw new InvalidOperationException("Already disposed") : _value;
            }
        }

        public bool Disposed { get; private set; }
        public void Dispose()
        {
            Disposed = true;
        }
    }

    public class MixedRequestHandler : IRequestHandler<MixedRequest>
    {
        private readonly MixedTaskHolder _holder;
        private readonly ScopeIdProvider _scopeIdProvider;

        public MixedRequestHandler(MixedTaskHolder holder, ScopeIdProvider scopeIdProvider)
        {
            _holder = holder;
            _scopeIdProvider = scopeIdProvider;
            holder.ScopeIdProviders.Add(scopeIdProvider);
        }
        public Task<Unit> Handle(MixedRequest request, CancellationToken cancellationToken)
        {
            _holder.Messages.Add("request");
            _holder.ScopeIds.Add(typeof(MixedRequestHandler), _scopeIdProvider.Value);
            return Unit.Task;
        }
    }

    public class MixedNotificationHandler : INotificationHandler<MixedNotification>
    {
        private readonly MixedTaskHolder _holder;
        private readonly ScopeIdProvider _scopeIdProvider;

        public MixedNotificationHandler(MixedTaskHolder holder, ScopeIdProvider scopeIdProvider)
        {
            _holder = holder;
            _scopeIdProvider = scopeIdProvider;
        }
        public Task Handle(MixedNotification request, CancellationToken cancellationToken)
        {
            _holder.Messages.Add("notification");
            _holder.ScopeIds.Add(typeof(MixedNotificationHandler), _scopeIdProvider.Value);
            return Task.CompletedTask;
        }
    }

    public class MixedNotificationHandler2 : INotificationHandler<MixedNotification>
    {
        private readonly MixedTaskHolder _holder;
        private readonly ScopeIdProvider _scopeIdProvider;

        public MixedNotificationHandler2(MixedTaskHolder holder, ScopeIdProvider scopeIdProvider)
        {
            _holder = holder;
            _scopeIdProvider = scopeIdProvider;
        }
        public Task Handle(MixedNotification request, CancellationToken cancellationToken)
        {
            _holder.Messages.Add("notification2");
            _holder.ScopeIds.Add(typeof(MixedNotificationHandler2), _scopeIdProvider.Value);
            return Task.CompletedTask;
        }
    }

    [AsEventListener(nameof(ExceptionPang))]
    public class ExceptionPang : INotification
    {
        public ExceptionPang(string value)
        {

        }
    }
    public class ExceptionPangHandler : INotificationHandler<ExceptionPang>
    {
        public Task Handle(ExceptionPang notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class LocalPong
    {
        public string Value { get; set; }
    }

    [AsEventListener(nameof(MediatorClientTestPang))]
    public class MediatorClientTestPang : INotification
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
            task = new Task(() =>
            {
                Thread.Sleep(10);
                holder.Messages.Add(message);
            });
            this.holder = holder;
            holder.Tasks.Add(task);
        }
        public Task Handle(MediatorClientTestPang notification, CancellationToken cancellationToken)
        {
            holder.Pangs.Add(notification);
            task.Start();
            // return Task.CompletedTask;
            return task;
        }
    }

    public class MediatorClientTestHandler1 : MediatorClientTestHandlerBase, INotificationHandler<MediatorClientTestPang>
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

    private class TestLogger : IServiceLikeLogger
    {
        public void Info(string message, params object[] args)
        {
            Infos.Add(message);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Errors.Add(message);
        }

        public void Trace(string message, params object[] args)
        {
            Traces.Add(message);
        }

        public bool IsTraceEnabled() => true;

        public bool IsInfoEnabled() => true;

        public bool IsErrorEnabled() => true;

        public List<string> Errors { get; } = new();

        public List<string> Infos { get; } = new();

        public List<string> Traces { get; } = new();
    }
}
