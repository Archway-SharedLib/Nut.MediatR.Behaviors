using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nut.MediatR.Test.RequestAware;
using Xunit;

namespace Nut.MediatR.Test.Logging;

public partial class LoggingBehaviorTest
{
    [Fact]
    public void ctor_ServiceFactory引数がnullの場合は例外が発生する()
    {
        var act = () => new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        act.Should().Throw<ArgumentNullException>();
    }

    //-----------------

    [Fact]
    public async Task Handle_Loggerが取得できなかった場合は処理が実行されずに終了する()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerNoLoggerRequest, LoggerNoLoggerRequest>>>(_ =>
        {
            return null;
        });

        var logging = new LoggingBehavior<LoggerNoLoggerRequest, LoggerNoLoggerRequest>(collection.BuildServiceProvider()); 
        var executed = false;
        await logging.Handle(new LoggerNoLoggerRequest(), () =>
        {
            executed = true;
            return Task.FromResult(new LoggerNoLoggerRequest());
        }, new CancellationToken());
        executed.Should().BeTrue();
    }

    public record LoggerNoLoggerRequest: IRequest<LoggerNoLoggerResponse>;
    public record LoggerNoLoggerResponse;

    public class LoggerNoLoggerHandler : IRequestHandler<LoggerNoLoggerRequest, LoggerNoLoggerResponse>
    {
        public Task<LoggerNoLoggerResponse> Handle(LoggerNoLoggerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new LoggerNoLoggerResponse());
        }
    }

    //-----------------

    [Fact]
    public async Task Handle_開始と終了でログが出力される()
    {
        var logger = new TestLogger<LoggingBehavior<LoggerOutStartEndRequest, LoggerOutStartEndResponse>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerOutStartEndRequest, LoggerOutStartEndResponse>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var r = await mediator.Send(new LoggerOutStartEndRequest());

        logger.Logs.Should().HaveCount(2);
        logger.Logs[0].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[0].Message.Should().Be($"Start {nameof(LoggerOutStartEndRequest)}.");
        logger.Logs[0].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndRequest));
        logger.Logs[1].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[1].Message.Should().MatchRegex($"Complete {nameof(LoggerOutStartEndRequest)} in [0-9]+ms.");
        logger.Logs[1].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Elapsed").Should().NotBeNull();
    }

    public record LoggerOutStartEndRequest : IRequest<LoggerOutStartEndResponse>;
    public record LoggerOutStartEndResponse;

    public class LoggerOutStartEndHandler(Executed executed) : IRequestHandler<LoggerOutStartEndRequest, LoggerOutStartEndResponse>
    {
        public Task<LoggerOutStartEndResponse> Handle(LoggerOutStartEndRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new LoggerOutStartEndResponse());
        }
    }

    //-----------------

    [Fact]
    public async Task Handle_InOutValueCollectorが設定されている場合は値も出力される()
    {
        var logger = new TestLogger<LoggingBehavior<LoggerOutStartEndWithInOutRequest, LoggerOutStartEndWithInOutResponse>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        collection.AddTransient<ILoggingInOutValueCollector<LoggerOutStartEndWithInOutRequest, LoggerOutStartEndWithInOutResponse>,
            LoggerOutStartEndInOutValueCollector>();
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerOutStartEndWithInOutRequest, LoggerOutStartEndWithInOutResponse>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var r = await mediator.Send(new LoggerOutStartEndWithInOutRequest());

        logger.Logs.Should().HaveCount(2);
        logger.Logs[0].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[0].Message.Should().Be($"Start {nameof(LoggerOutStartEndWithInOutRequest)}. In");
        logger.Logs[0].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndWithInOutRequest));
        logger.Logs[0].State.First(kv => kv.Key == "Input").Value.ToString().Should().Be("In");
        logger.Logs[1].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[1].Message.Should().MatchRegex($"Complete {nameof(LoggerOutStartEndWithInOutRequest)} in [0-9]+ms. Out");
        logger.Logs[1].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndWithInOutRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Elapsed").Should().NotBeNull();
        logger.Logs[1].State.First(kv => kv.Key == "Output").Value.ToString().Should().Be("Out");
    }

    public record LoggerOutStartEndWithInOutRequest : IRequest<LoggerOutStartEndWithInOutResponse>;
    public record LoggerOutStartEndWithInOutResponse;

    public class LoggerOutStartWithInOutEndHandler(Executed executed) : IRequestHandler<LoggerOutStartEndWithInOutRequest, LoggerOutStartEndWithInOutResponse>
    {
        public Task<LoggerOutStartEndWithInOutResponse> Handle(LoggerOutStartEndWithInOutRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new LoggerOutStartEndWithInOutResponse());
        }
    }

    public class LoggerOutStartEndInOutValueCollector: ILoggingInOutValueCollector<LoggerOutStartEndWithInOutRequest, LoggerOutStartEndWithInOutResponse>
    {
        public Task<InOutValueResult> CollectInValueAsync(LoggerOutStartEndWithInOutRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult.WithValue("In"));
        }

        public Task<InOutValueResult> CollectOutValueAsync(LoggerOutStartEndWithInOutResponse response, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult.WithValue("Out"));
        }
    }

    //-----------------

    [Fact]
    public async Task Handle_InOutValueCollectorが設定されていても結果がEmptyの場合は値は出力されない()
    {
        var logger = new TestLogger<LoggingBehavior<LoggerOutStartEndWithEmptyInOutRequest, LoggerOutStartEndWithEmptyInOutResponse>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        collection.AddTransient<ILoggingInOutValueCollector<LoggerOutStartEndWithEmptyInOutRequest, LoggerOutStartEndWithEmptyInOutResponse>,
            LoggerOutStartEndEmptyInOutValueCollector>();
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerOutStartEndWithEmptyInOutRequest, LoggerOutStartEndWithEmptyInOutResponse>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var r = await mediator.Send(new LoggerOutStartEndWithEmptyInOutRequest());

        logger.Logs.Should().HaveCount(2);
        logger.Logs[0].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[0].Message.Should().Be($"Start {nameof(LoggerOutStartEndWithEmptyInOutRequest)}.");
        logger.Logs[0].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndWithEmptyInOutRequest));
        logger.Logs[0].State.Any(kv => kv.Key == "Input").Should().BeFalse();
        logger.Logs[1].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[1].Message.Should().MatchRegex($"Complete {nameof(LoggerOutStartEndWithEmptyInOutRequest)} in [0-9]+ms.");
        logger.Logs[1].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndWithEmptyInOutRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Elapsed").Should().NotBeNull();
        logger.Logs[1].State.Any(kv => kv.Key == "Output").Should().BeFalse();
    }

    public record LoggerOutStartEndWithEmptyInOutRequest : IRequest<LoggerOutStartEndWithEmptyInOutResponse>;
    public record LoggerOutStartEndWithEmptyInOutResponse;

    public class LoggerOutStartWithEmptyInOutEndHandler(Executed executed) : IRequestHandler<LoggerOutStartEndWithEmptyInOutRequest, LoggerOutStartEndWithEmptyInOutResponse>
    {
        public Task<LoggerOutStartEndWithEmptyInOutResponse> Handle(LoggerOutStartEndWithEmptyInOutRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new LoggerOutStartEndWithEmptyInOutResponse());
        }
    }

    public class LoggerOutStartEndEmptyInOutValueCollector : ILoggingInOutValueCollector<LoggerOutStartEndWithEmptyInOutRequest, LoggerOutStartEndWithEmptyInOutResponse>
    {
        public Task<InOutValueResult> CollectInValueAsync(LoggerOutStartEndWithEmptyInOutRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult.Empty());
        }

        public Task<InOutValueResult> CollectOutValueAsync(LoggerOutStartEndWithEmptyInOutResponse response, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult.Empty());
        }
    }

    //-----------------

    [Fact]
    public async Task Handle_InOutValueCollectorが設定されていても結果がNullの場合は値は出力されない()
    {
        var logger = new TestLogger<LoggingBehavior<LoggerOutStartEndWithNullInOutRequest, LoggerOutStartEndWithNullInOutResponse>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        collection.AddTransient<ILoggingInOutValueCollector<LoggerOutStartEndWithNullInOutRequest, LoggerOutStartEndWithNullInOutResponse>,
            LoggerOutStartEndNullInOutValueCollector>();
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerOutStartEndWithNullInOutRequest, LoggerOutStartEndWithNullInOutResponse>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var r = await mediator.Send(new LoggerOutStartEndWithNullInOutRequest());

        logger.Logs.Should().HaveCount(2);
        logger.Logs[0].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[0].Message.Should().Be($"Start {nameof(LoggerOutStartEndWithNullInOutRequest)}.");
        logger.Logs[0].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndWithNullInOutRequest));
        logger.Logs[0].State.Any(kv => kv.Key == "Input").Should().BeFalse();
        logger.Logs[1].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[1].Message.Should().MatchRegex($"Complete {nameof(LoggerOutStartEndWithNullInOutRequest)} in [0-9]+ms.");
        logger.Logs[1].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerOutStartEndWithNullInOutRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Elapsed").Should().NotBeNull();
        logger.Logs[1].State.Any(kv => kv.Key == "Output").Should().BeFalse();
    }

    public record LoggerOutStartEndWithNullInOutRequest : IRequest<LoggerOutStartEndWithNullInOutResponse>;
    public record LoggerOutStartEndWithNullInOutResponse;

    public class LoggerOutStartWithNullInOutEndHandler(Executed executed) : IRequestHandler<LoggerOutStartEndWithNullInOutRequest, LoggerOutStartEndWithNullInOutResponse>
    {
        public Task<LoggerOutStartEndWithNullInOutResponse> Handle(LoggerOutStartEndWithNullInOutRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new LoggerOutStartEndWithNullInOutResponse());
        }
    }

    public class LoggerOutStartEndNullInOutValueCollector : ILoggingInOutValueCollector<LoggerOutStartEndWithNullInOutRequest, LoggerOutStartEndWithNullInOutResponse>
    {
        public Task<InOutValueResult> CollectInValueAsync(LoggerOutStartEndWithNullInOutRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult.Empty());
        }

        public Task<InOutValueResult> CollectOutValueAsync(LoggerOutStartEndWithNullInOutResponse response, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult.Empty());
        }
    }

    //---------------------

    [Fact]
    public async Task Handle_例外が発生した場合は例外も出力しそのままリスローする()
    {
        var logger = new TestLogger<LoggingBehavior<LoggerExceptionRequest, LoggerExceptionResponse>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerExceptionRequest, LoggerExceptionResponse>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var act = () => mediator.Send(new LoggerExceptionRequest());
        await act.Should().ThrowAsync<Exception>().WithMessage("Dummy Exception");

        logger.Logs.Should().HaveCount(2);
        logger.Logs[0].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[0].Message.Should().Be($"Start {nameof(LoggerExceptionRequest)}.");
        logger.Logs[0].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerExceptionRequest));
        logger.Logs[0].Exception.Should().BeNull();
        logger.Logs[1].LogLevel.Should().Be(LogLevel.Error);
        logger.Logs[1].Message.Should().MatchRegex($"Exception {nameof(LoggerExceptionRequest)} in [0-9]+ms.");
        logger.Logs[1].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerExceptionRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Elapsed").Should().NotBeNull();
        logger.Logs[1].Exception.Should().NotBeNull().And.BeOfType<Exception>().Subject.Message.Should().Be("Dummy Exception");
    }

    public record LoggerExceptionRequest : IRequest<LoggerExceptionResponse>;
    public record LoggerExceptionResponse;

    public class LoggerExceptionHandler : IRequestHandler<LoggerExceptionRequest, LoggerExceptionResponse>
    {
        public Task<LoggerExceptionResponse> Handle(LoggerExceptionRequest request, CancellationToken cancellationToken)
        {
            throw new Exception("Dummy Exception");
            // return Task.FromResult(new LoggerExceptionResponse());
        }
    }

    //---------------------

    [Fact]
    public async Task Handle_LoggingBehaviorを継承してデフォルトのInOutCollectorを指定できる()
    {
        var logger = new TestLogger<LoggingBehavior<LoggerInheritRequest, LoggerInheritResponse>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(InheritLoggingBehavior<,>));
        });
        collection.AddTransient<ILoggingInOutValueCollector<LoggerInheritRequest, LoggerInheritResponse>,
            InheritInOutValueCollector<LoggerInheritRequest, LoggerInheritResponse>>();
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerInheritRequest, LoggerInheritResponse>>>(logger);
        collection.AddSingleton<ExecHistory>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var r = await mediator.Send(new LoggerInheritRequest());

        logger.Logs.Should().HaveCount(2);
        logger.Logs[0].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[0].Message.Should().Be($"Start {nameof(LoggerInheritRequest)}. Inherit In");
        logger.Logs[0].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerInheritRequest));
        logger.Logs[0].State.First(kv => kv.Key == "Input").Value.ToString().Should().Be("Inherit In");
        logger.Logs[1].LogLevel.Should().Be(LogLevel.Information);
        logger.Logs[1].Message.Should().MatchRegex($"Complete {nameof(LoggerInheritRequest)} in [0-9]+ms. Inherit Out");
        logger.Logs[1].State.First(kv => kv.Key == "Request").Value.ToString().Should().Be(nameof(LoggerInheritRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Elapsed").Should().NotBeNull();
        logger.Logs[1].State.First(kv => kv.Key == "Output").Value.ToString().Should().Be("Inherit Out");

        var executed = provider.GetService<ExecHistory>();
        executed.List.Should().HaveCount(2);
        executed.List[0].Should().Be("In");
        executed.List[1].Should().Be("Out");
    }

    public record LoggerInheritRequest : IRequest<LoggerInheritResponse>;
    public record LoggerInheritResponse;

    public class LoggerInheritHandler : IRequestHandler<LoggerInheritRequest, LoggerInheritResponse>
    {
        public Task<LoggerInheritResponse> Handle(LoggerInheritRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new LoggerInheritResponse());
        }
    }

    public class InheritLoggingBehavior<TRequest, TResponse>(IServiceProvider serviceProvider, ExecHistory executed) : LoggingBehavior<TRequest, TResponse>(serviceProvider) where TRequest : notnull
    {
        protected override ILoggingInOutValueCollector<TRequest, TResponse> GetDefaultCollector() => new InheritInOutValueCollector<TRequest, TResponse>(executed);
    }

    public class InheritInOutValueCollector<LoggerInheritRequest, LoggerInheritResponse>(ExecHistory executed): ILoggingInOutValueCollector<LoggerInheritRequest, LoggerInheritResponse>
    {
        public Task<InOutValueResult> CollectInValueAsync(LoggerInheritRequest request, CancellationToken cancellationToken)
        {
            executed.List.Add("In");
            return Task.FromResult(InOutValueResult.WithValue("Inherit In"));
        }

        public Task<InOutValueResult> CollectOutValueAsync(LoggerInheritResponse response, CancellationToken cancellationToken)
        {
            executed.List.Add("Out");
            return Task.FromResult(InOutValueResult.WithValue("Inherit Out"));
        }
    }
}
