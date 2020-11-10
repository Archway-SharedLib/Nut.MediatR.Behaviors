using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Nut.MediatR.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Nut.MediatR.Test.Logging
{
    public class LoggingBehaviorTest
    {
        [Fact]
        public void ctor_ILoggerFactory引数がnullの場合は例外が発生する()
        {
            Action act = () => new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(null, new ServiceFactory(_ => null));
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_ServiceFactory引数がnullの場合は例外が発生する()
        {
            var lf = Substitute.For<ILoggerFactory>();
            Action act = () => new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(lf, null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Handle_開始と終了でログが出力される()
        {
            var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();
            var lf = Substitute.For<ILoggerFactory>();
            lf.CreateLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>()
                .Returns(logger);

            var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(lf, new ServiceFactory(_ => null));
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
            var lf = Substitute.For<ILoggerFactory>();
            lf.CreateLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>()
                .Returns(logger);

            var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(lf, 
                new ServiceFactory(_ => new TestLoggingInOutValueCollector1()));
            await logging.Handle(new TestBehaviorRequest() { Value = "A"}, new CancellationToken(), () =>
            {
                logger.Logs.Should().HaveCount(1);
                var logInfo = logger.Logs[0];
                var input = logInfo.State.First(s => s.Key == "Input").Value.Should().Be("A");

                return Task.FromResult(new TestBehaviorResponse() { Value = "B"});
            });
            logger.Logs.Should().HaveCount(2);
            var logInfo = logger.Logs[1];

            var output = logInfo.State.First(s => s.Key == "Output").Value.Should().Be("B");
        }

        [Fact]
        public async Task Handle_InOutValueCollectorが設定されていても結果がEmptyの場合は値は出力されない()
        {
            var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();
            var lf = Substitute.For<ILoggerFactory>();
            lf.CreateLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>()
                .Returns(logger);

            var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(lf,
                new ServiceFactory(_ => new TestLoggingInOutValueCollector2()));
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
            var lf = Substitute.For<ILoggerFactory>();
            lf.CreateLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>()
                .Returns(logger);

            var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(lf,
                new ServiceFactory(_ => new TestLoggingInOutValueCollector3()));
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
        public void Handle_例外が発生した場合は例外も出力しそのままリスローする()
        {
            var logger = new TestLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>();
            var lf = Substitute.For<ILoggerFactory>();
            lf.CreateLogger<LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>>()
                .Returns(logger);

            var logging = new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(lf, new ServiceFactory(_ => null));
            Func<Task> act = () => logging.Handle(new TestBehaviorRequest() { Value = "A" }, new CancellationToken(), () =>
            {
                logger.Logs.Should().HaveCount(1);
                throw new InvalidOperationException();
            });

            act.Should().Throw<InvalidOperationException>();

            logger.Logs.Should().HaveCount(2);
            var logInfo = logger.Logs[1];

            logInfo.Exception.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
            logInfo.State.First(s => s.Key == "Request").Value.Should().Be(typeof(TestBehaviorRequest).Name);
            logInfo.State.Any(s => s.Key == "Elapsed").Should().BeTrue();
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

}
