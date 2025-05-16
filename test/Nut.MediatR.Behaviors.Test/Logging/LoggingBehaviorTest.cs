using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nut.MediatR.Test.RequestAware;
using Xunit;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nut.MediatR.Test.Logging;

public partial class LoggingBehaviorTest
{
    [Fact]
    public void ctor_ServiceFactory引数がnullの場合は例外が発生する()
    {
        var act = () => new LoggingBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        Should.Throw<ArgumentNullException>(act);
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
        await logging.Handle(new LoggerNoLoggerRequest(), (_) =>
        {
            executed = true;
            return Task.FromResult(new LoggerNoLoggerRequest());
        }, CancellationToken.None);
        executed.ShouldBeTrue();
    }

    public record LoggerNoLoggerRequest : IRequest<LoggerNoLoggerResponse>;
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

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(LoggerOutStartEndRequest)}.");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndRequest));
        Regex.IsMatch(logger.Logs[1].Message, $"Complete {nameof(LoggerOutStartEndRequest)} in [0-9]+ms.").ShouldBeTrue();
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Mediator.Elapsed").ShouldNotBe(default(KeyValuePair<string, object>));
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

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(LoggerOutStartEndWithInOutRequest)}. value: In");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndWithInOutRequest));
        logger.Logs[0].State.First(kv => kv.Key == "value").Value.ToString().ShouldBe("In");
        Regex.IsMatch(logger.Logs[1].Message, $"Complete {nameof(LoggerOutStartEndWithInOutRequest)} in [0-9]+ms. value: Out").ShouldBeTrue();
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndWithInOutRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Mediator.Elapsed").ShouldNotBe(default(KeyValuePair<string, object>));
        logger.Logs[1].State.First(kv => kv.Key == "value").Value.ToString().ShouldBe("Out");
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

    public class LoggerOutStartEndInOutValueCollector : ILoggingInOutValueCollector<LoggerOutStartEndWithInOutRequest, LoggerOutStartEndWithInOutResponse>
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

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(LoggerOutStartEndWithEmptyInOutRequest)}.");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndWithEmptyInOutRequest));
        logger.Logs[0].State.Any(kv => kv.Key == "value").ShouldBeFalse();
        Regex.IsMatch(logger.Logs[1].Message, $"Complete {nameof(LoggerOutStartEndWithEmptyInOutRequest)} in [0-9]+ms.").ShouldBeTrue();
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndWithEmptyInOutRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Mediator.Elapsed").ShouldNotBe(default(KeyValuePair<string, object>));
        logger.Logs[1].State.Any(kv => kv.Key == "value").ShouldBeFalse();
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
        var ex = await Should.ThrowAsync<Exception>(act);
        ex.Message.ShouldBe("Dummy Exception");

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(LoggerExceptionRequest)}.");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerExceptionRequest));
        logger.Logs[0].Exception.ShouldBeNull();
        logger.Logs[1].LogLevel.ShouldBe(LogLevel.Error);
        Regex.IsMatch(logger.Logs[1].Message, $"Exception {nameof(LoggerExceptionRequest)} in [0-9]+ms.").ShouldBeTrue();
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerExceptionRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Mediator.Elapsed").ShouldNotBe(default(KeyValuePair<string, object>));
        logger.Logs[1].Exception.ShouldNotBeNull();
        logger.Logs[1].Exception.ShouldBeOfType<Exception>().Message.ShouldBe("Dummy Exception");
    }

    public record LoggerExceptionRequest : IRequest<LoggerExceptionResponse>;
    public record LoggerExceptionResponse;

    public class LoggerExceptionHandler : IRequestHandler<LoggerExceptionRequest, LoggerExceptionResponse>
    {
        public Task<LoggerExceptionResponse> Handle(LoggerExceptionRequest request, CancellationToken cancellationToken)
        {
            throw new Exception("Dummy Exception");
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

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(LoggerInheritRequest)}. value: Inherit In");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerInheritRequest));
        logger.Logs[0].State.First(kv => kv.Key == "value").Value.ToString().ShouldBe("Inherit In");
        Regex.IsMatch(logger.Logs[1].Message, $"Complete {nameof(LoggerInheritRequest)} in [0-9]+ms. value: Inherit Out").ShouldBeTrue();
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerInheritRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Mediator.Elapsed").ShouldNotBe(default(KeyValuePair<string, object>));
        logger.Logs[1].State.First(kv => kv.Key == "value").Value.ToString().ShouldBe("Inherit Out");

        var executed = provider.GetService<ExecHistory>();
        executed.List.Count.ShouldBe(2);
        executed.List[0].ShouldBe("In");
        executed.List[1].ShouldBe("Out");
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

    public class InheritInOutValueCollector<LoggerInheritRequest, LoggerInheritResponse>(ExecHistory executed) : ILoggingInOutValueCollector<LoggerInheritRequest, LoggerInheritResponse>
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

    //--------

    [Fact]
    public async Task Handle_InOutValueCollectorで複数の値が設定されている場合は複数の値が出力される()
    {
        var logger = new TestLogger<LoggingBehavior<LoggerOutStartEndWithMultipleValueInOutRequest, LoggerOutStartEndWithMultipleValueInOutResponse>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(RequestAwareBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        collection.AddTransient<ILoggingInOutValueCollector<LoggerOutStartEndWithMultipleValueInOutRequest, LoggerOutStartEndWithMultipleValueInOutResponse>,
            LoggerOutStartEndInOutMultipleValueCollector>();
        collection.AddSingleton<ILogger<LoggingBehavior<LoggerOutStartEndWithMultipleValueInOutRequest, LoggerOutStartEndWithMultipleValueInOutResponse>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var r = await mediator.Send(new LoggerOutStartEndWithMultipleValueInOutRequest());

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(LoggerOutStartEndWithMultipleValueInOutRequest)}. val: val-value, numvalue: 123, boolvalue: True");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndWithMultipleValueInOutRequest));
        logger.Logs[0].State.First(kv => kv.Key == "val").Value.ToString().ShouldBe("val-value");
        logger.Logs[0].State.First(kv => kv.Key == "numvalue").Value.ShouldBe(123);
        logger.Logs[0].State.First(kv => kv.Key == "boolvalue").Value.ShouldBe(true);

        Regex.IsMatch(logger.Logs[1].Message, $"Complete {nameof(LoggerOutStartEndWithMultipleValueInOutRequest)} in [0-9]+ms\\. val: val\\-value, numvalue: 123, boolvalue: True").ShouldBeTrue();
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(LoggerOutStartEndWithMultipleValueInOutRequest));
        logger.Logs[1].State.FirstOrDefault(kv => kv.Key == "Mediator.Elapsed").ShouldNotBe(default(KeyValuePair<string, object>));
        logger.Logs[1].State.First(kv => kv.Key == "val").Value.ToString().ShouldBe("val-value");
        logger.Logs[1].State.First(kv => kv.Key == "numvalue").Value.ShouldBe(123);
        logger.Logs[1].State.First(kv => kv.Key == "boolvalue").Value.ShouldBe(true);
    }

    public record LoggerOutStartEndWithMultipleValueInOutRequest : IRequest<LoggerOutStartEndWithMultipleValueInOutResponse>;
    public record LoggerOutStartEndWithMultipleValueInOutResponse;

    public class LoggerOutStartWithMultipleValueInOutEndHandler(Executed executed) : IRequestHandler<LoggerOutStartEndWithMultipleValueInOutRequest, LoggerOutStartEndWithMultipleValueInOutResponse>
    {
        public Task<LoggerOutStartEndWithMultipleValueInOutResponse> Handle(LoggerOutStartEndWithMultipleValueInOutRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new LoggerOutStartEndWithMultipleValueInOutResponse());
        }
    }

    public class LoggerOutStartEndInOutMultipleValueCollector : ILoggingInOutValueCollector<LoggerOutStartEndWithMultipleValueInOutRequest, LoggerOutStartEndWithMultipleValueInOutResponse>
    {
        public Task<InOutValueResult> CollectInValueAsync(LoggerOutStartEndWithMultipleValueInOutRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult
                .WithKeyValue("val", "val-value")
                .Add("numvalue", 123)
                .Add("boolvalue", true));
        }

        public Task<InOutValueResult> CollectOutValueAsync(LoggerOutStartEndWithMultipleValueInOutResponse response, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult
                .WithKeyValue("val", "val-value")
                .Add("numvalue", 123)
                .Add("boolvalue", true));
        }
    }

}
