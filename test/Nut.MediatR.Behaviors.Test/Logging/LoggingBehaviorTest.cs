using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Nut.MediatR.Test.Logging;

public class LoggingBehaviorTest
{
    [Fact]
    public void ctor_ServiceFactory引数がnullの場合は例外が発生する()
    {
        var lf = Substitute.For<ILoggerFactory>();
        Action act = () => new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_Loggerが取得できなかった場合は処理が実行されずに終了する()
    {
        var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();
        var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(new ServiceFactory(_ => null));

        var executed = false;
        await logging.Handle(new TestBehaviorRequest(), new CancellationToken(), () =>
        {
            executed = true;
            return Task.FromResult(new TestBehaviorResponse());
        });
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_開始と終了でログが出力される()
    {
        var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();

        var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(new ServiceFactory(type =>
        {
            return type.GetGenericTypeDefinition() == typeof(ILogger<>) ? logger : null;
        }));
        await logging.Handle(new TestBehaviorRequest(), new CancellationToken(), () =>
        {
            logger.Logs.Should().HaveCount(1);
            var logInfo = logger.Logs[0];
            logInfo.LogLevel.Should().Be(LogLevel.Information);
            logInfo.State.First(s => s.Key == "Request").Value.Should().Be(typeof(TestBehaviorRequest).Name);
            logInfo.State.Any(s => s.Key == "Input").Should().BeFalse();
            return Task.FromResult(new TestBehaviorResponse());
        });
        logger.Logs.Should().HaveCount(2);
        var logInfo = logger.Logs[1];
        logInfo.LogLevel.Should().Be(LogLevel.Information);
        logInfo.State.First(s => s.Key == "Request").Value.Should().Be(typeof(TestBehaviorRequest).Name);
        logInfo.State.Any(s => s.Key == "Elapsed").Should().BeTrue();
        logInfo.State.Any(s => s.Key == "Output").Should().BeFalse();
    }

    [Fact]
    public async Task Handle_InOutValueCollectorが設定されている場合は値も出力される()
    {
        var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();

        var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            new ServiceFactory(type =>
            {
                return type.GetGenericTypeDefinition() == typeof(ILogger<>) ? (object)logger : (object)(new TestLoggingInOutValueCollector1());
            }));
        await logging.Handle(new TestBehaviorRequest() { Value = "A" }, new CancellationToken(), () =>
         {
             logger.Logs.Should().HaveCount(1);
             var logInfo = logger.Logs[0];
             var input = logInfo.State.First(s => s.Key == "Input").Value.Should().Be("A");

             return Task.FromResult(new TestBehaviorResponse() { Value = "B" });
         });
        logger.Logs.Should().HaveCount(2);
        var logInfo = logger.Logs[1];

        var output = logInfo.State.First(s => s.Key == "Output").Value.Should().Be("B");
    }

    [Fact]
    public async Task Handle_InOutValueCollectorが設定されていても結果がEmptyの場合は値は出力されない()
    {
        var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();

        var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            new ServiceFactory(type =>
            {
                return type.GetGenericTypeDefinition() == typeof(ILogger<>) ? (object)logger : (object)(new TestLoggingInOutValueCollector2());
            }));
        await logging.Handle(new TestBehaviorRequest() { Value = "A" }, new CancellationToken(), () =>
        {
            logger.Logs.Should().HaveCount(1);
            var logInfo = logger.Logs[0];
            var input = logInfo.State.Any(s => s.Key == "Input").Should().BeFalse();

            return Task.FromResult(new TestBehaviorResponse() { Value = "B" });
        });
        logger.Logs.Should().HaveCount(2);
        var logInfo = logger.Logs[1];

        var output = logInfo.State.Any(s => s.Key == "Output").Should().BeFalse();
    }

    [Fact]
    public async Task Handle_InOutValueCollectorが設定されていても結果がNullの場合は値は出力されない()
    {
        var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();

        var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            new ServiceFactory(type =>
            {
                return type.GetGenericTypeDefinition() == typeof(ILogger<>) ? (object)logger : (object)(new TestLoggingInOutValueCollector3());
            }));
        await logging.Handle(new TestBehaviorRequest() { Value = "A" }, new CancellationToken(), () =>
        {
            logger.Logs.Should().HaveCount(1);
            var logInfo = logger.Logs[0];
            var input = logInfo.State.Any(s => s.Key == "Input").Should().BeFalse();

            return Task.FromResult(new TestBehaviorResponse() { Value = "B" });
        });
        logger.Logs.Should().HaveCount(2);
        var logInfo = logger.Logs[1];

        var output = logInfo.State.Any(s => s.Key == "Output").Should().BeFalse();
    }

    [Fact]
    public async Task Handle_例外が発生した場合は例外も出力しそのままリスローする()
    {
        var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();

        var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(new ServiceFactory(type =>
        {
            return type.GetGenericTypeDefinition() == typeof(ILogger<>) ? logger : null;
        }));
        Func<Task> act = () => logging.Handle(new TestBehaviorRequest() { Value = "A" }, new CancellationToken(), () =>
        {
            logger.Logs.Should().HaveCount(1);
            throw new InvalidOperationException();
        });

        await act.Should().ThrowAsync<InvalidOperationException>();

        logger.Logs.Should().HaveCount(2);
        var logInfo = logger.Logs[1];

        logInfo.Exception.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        logInfo.State.First(s => s.Key == "Request").Value.Should().Be(typeof(TestBehaviorRequest).Name);
        logInfo.State.Any(s => s.Key == "Elapsed").Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ServiceFacotryからCollectorが取得できないときにDefaultCollectorが利用される()
    {
        var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();

        var logging = new TestLoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(new ServiceFactory(type =>
        {
            return type.GetGenericTypeDefinition() == typeof(ILogger<>) ? logger : null;
        }));

        logging.Collector.ExecutedInValueAsync.Should().BeFalse();
        logging.Collector.ExecutedOutValueAsync.Should().BeFalse();
        logging.ExecutedGetDefaultCollector.Should().BeFalse();
        await logging.Handle(new TestBehaviorRequest() { Value = "A" }, new CancellationToken(), () =>
        {
            logging.Collector.ExecutedInValueAsync.Should().BeTrue();
            logging.Collector.ExecutedOutValueAsync.Should().BeFalse();
            logging.ExecutedGetDefaultCollector.Should().BeTrue();
            return Task.FromResult(new TestBehaviorResponse() { Value = "B" });
        });
        logging.Collector.ExecutedInValueAsync.Should().BeTrue();
        logging.Collector.ExecutedOutValueAsync.Should().BeTrue();
        logging.ExecutedGetDefaultCollector.Should().BeTrue();
    }
}

public class TestLoggingBehavior<TRequest, TResponse> : LoggingBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
{
    public TestLoggingInOutValueCollector<TRequest, TResponse> Collector { get; } = new TestLoggingInOutValueCollector<TRequest, TResponse>();

    public bool ExecutedGetDefaultCollector { get; private set; } = false;

    public TestLoggingBehavior(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    protected override ILoggingInOutValueCollector<TRequest, TResponse> GetDefaultCollector()
    {
        ExecutedGetDefaultCollector = true;
        return Collector;
    }
}

public class TestLoggingInOutValueCollector<TRequest, TResponse> : ILoggingInOutValueCollector<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public bool ExecutedInValueAsync { get; private set; } = false;

    public bool ExecutedOutValueAsync { get; private set; } = false;

    public Task<InOutValueResult> CollectInValueAsync(TRequest request, CancellationToken cancellationToken)
    {
        ExecutedInValueAsync = true;
        return Task.FromResult(InOutValueResult.WithValue(request));
    }

    public Task<InOutValueResult> CollectOutValueAsync(TResponse response, CancellationToken cancellationToken)
    {
        ExecutedOutValueAsync = true;
        return Task.FromResult(InOutValueResult.WithValue(response));
    }
}

public class TestLoggingInOutValueCollector1 : ILoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse>
{
    public Task<InOutValueResult> CollectInValueAsync(TestBehaviorRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(request.Value));
    }

    public Task<InOutValueResult> CollectOutValueAsync(TestBehaviorResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(response.Value));
    }
}

public class TestLoggingInOutValueCollector2 : ILoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse>
{
    public Task<InOutValueResult> CollectInValueAsync(TestBehaviorRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.Empty());
    }

    public Task<InOutValueResult> CollectOutValueAsync(TestBehaviorResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.Empty());
    }
}

public class TestLoggingInOutValueCollector3 : ILoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse>
{
    public Task<InOutValueResult> CollectInValueAsync(TestBehaviorRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(null as InOutValueResult);
    }

    public Task<InOutValueResult> CollectOutValueAsync(TestBehaviorResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(null as InOutValueResult);
    }
}
