using BehaviorSample.Sample;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nut.MediatR;
using System;
using System.Threading.Tasks;

namespace BehaviorSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = new ServiceCollection()
                .AddLogging(config =>
                {
                    config.AddConsole();
                }).AddMediatR(typeof(Program))
                // MediatRから呼べるようにPreRequestBehaviorを登録する
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerRequestBehavior<,>))
                // PreRequestBehaviorから利用するBehaviorは直接型でインスタンスを取得するので、IPipelineBehavior経由にしない。
                .AddTransient(typeof(LoggingBehavior<,>))
                .AddTransient(typeof(AuthorizationBehavior<,>))
                .AddTransient(typeof(DataAnnotationValidationBehavior<,>))
                // IAuthorizerやILoggingInOutValueCollectorはアセンブリをスキャンして登録すると便利
                .Scan(scan => scan
                    .FromAssemblyOf<Program>()
                    .AddClasses(cls => cls.AssignableTo(typeof(IAuthorizer<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime())
                .Scan(scan => scan
                    .FromAssemblyOf<Program>()
                    .AddClasses(cls => cls.AssignableTo(typeof(ILoggingInOutValueCollector<,>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime())
                .BuildServiceProvider();

            var result = await provider.GetService<IMediator>().Send(new SampleRequest() { Value = "Hello" });

            Console.WriteLine(result.Value);

        }
    }
}
