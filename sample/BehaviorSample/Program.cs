using System;
using System.Threading.Tasks;
using BehaviorSample.Sample;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nut.MediatR;

namespace BehaviorSample;

class Program
{
    static async Task Main(string[] args)
    {
        var provider = new ServiceCollection()
            .AddLogging(config =>
            {
                config.AddConsole();
            })
            .AddValidatorsFromAssemblies(new[] { typeof(Program).Assembly })
            .AddMediatR(cfg => {
                cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly)
                    .AddOpenBehavior(typeof(PerRequestBehavior<,>));
            })
            // PreRequestBehaviorから利用するBehaviorは直接型でインスタンスを取得するので、IPipelineBehavior経由にしない。
            .AddTransient(typeof(LoggingBehavior<,>))
            .AddTransient(typeof(AuthorizationBehavior<,>))
            .AddTransient(typeof(DataAnnotationValidationBehavior<,>))
            .AddTransient(typeof(FluentValidationBehavior<,>))
            // IAuthorizerやILoggingInOutValueCollectorはアセンブリをスキャンして登録すると便利
            .Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(cls =>
                    cls.AssignableTo(typeof(IAuthorizer<>))
                    .Where(type => !type.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime())
            .Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(cls =>
                    cls.AssignableTo(typeof(ILoggingInOutValueCollector<,>))
                    .Where(type => !type.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime())
            .BuildServiceProvider();

        var result = await provider.GetService<IMediator>().Send(new SampleRequest() { Value = "Hello" });

        Console.WriteLine(result.Value);

    }
}
