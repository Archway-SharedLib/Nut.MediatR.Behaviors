using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Nut.MediatR.Test.Logging;
public partial class LoggingBehaviorTest
{
    [Fact]
    public async Task Handle_Void_ログが出力される()
    {
        var logger = new TestLogger<LoggingBehavior<SimpleVoidRequest, Unit>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(LoggingBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        collection.AddTransient<ILoggingInOutValueCollector<SimpleVoidRequest>, SimpleVoidInOutValueCollector>();
        collection.AddSingleton<ILogger<LoggingBehavior<SimpleVoidRequest, Unit>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        await mediator.Send(new SimpleVoidRequest());

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(SimpleVoidRequest)}. value: Simple InValue");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(SimpleVoidRequest));
        logger.Logs[1].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[1].Message.ShouldMatch($"Complete {nameof(SimpleVoidRequest)} in [0-9]+ms.");
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(SimpleVoidRequest));
        logger.Logs[1].State.Any(kv => kv.Key == "   Elapsed").ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_Void_InOutValueCollectorがResponseのUnit付きで登録されていても読める()
    {
        var logger = new TestLogger<LoggingBehavior<SimpleVoidRequest, Unit>>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(LoggingBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        collection.AddTransient<ILoggingInOutValueCollector<SimpleVoidRequest, Unit>, SimpleVoidInOutValueCollector>();
        collection.AddSingleton<ILogger<LoggingBehavior<SimpleVoidRequest, Unit>>>(logger);
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        await mediator.Send(new SimpleVoidRequest());

        logger.Logs.Count.ShouldBe(2);
        logger.Logs[0].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[0].Message.ShouldBe($"Start {nameof(SimpleVoidRequest)}. value: Simple InValue");
        logger.Logs[0].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(SimpleVoidRequest));
        logger.Logs[1].LogLevel.ShouldBe(LogLevel.Information);
        logger.Logs[1].Message.ShouldMatch($"Complete {nameof(SimpleVoidRequest)} in [0-9]+ms.");
        logger.Logs[1].State.First(kv => kv.Key == "Mediator.Request").Value.ToString().ShouldBe(nameof(SimpleVoidRequest));
        logger.Logs[1].State.Any(kv => kv.Key == "Mediator.Elapsed").ShouldBeTrue();
    }

    public record SimpleVoidRequest : IRequest;

    public class SimpleVoidRequestHandler : IRequestHandler<SimpleVoidRequest>
    {
        public Task Handle(SimpleVoidRequest request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class SimpleVoidInOutValueCollector : ILoggingInOutValueCollector<SimpleVoidRequest>
    {
        public Task<InOutValueResult> CollectInValueAsync(SimpleVoidRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(InOutValueResult.WithValue("Simple InValue"));
        }
    }
}
