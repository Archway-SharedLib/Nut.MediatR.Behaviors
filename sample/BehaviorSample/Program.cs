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
    static async Task Main()
    {
        var provider = new ServiceCollection()
            .AddLogging(config =>
            {
                config.AddConsole();
            })
            .AddValidatorsFromAssemblies(new[] { typeof(Program).Assembly })
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            })
            .AddMediatRRequestAwareBehavior(builder =>
            {
                // 各サービスをもっているアセンブリを登録する

                builder
                    .AddAssembliesForAutoRegister(typeof(Program).Assembly)
                    .AddLogging()
                    .AddAuthorization()
                    .AddDataAnnotationValidation()
                    .AddOpenBehavior(typeof(FluentValidationBehavior<,>));
            })
            // PreRequestBehaviorから利用するBehaviorは直接型でインスタンスを取得するので、IPipelineBehavior経由にしない。
            // IAuthorizerやILoggingInOutValueCollectorはアセンブリをスキャンして登録すると便利
            .Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(cls =>
                    cls.AssignableTo(typeof(IAuthorizer<>))
                    .Where(type => !type.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime())
            //.Scan(scan => scan
            //    .FromAssemblyOf<Program>()
            //    .AddClasses(cls =>
            //        cls.AssignableTo(typeof(ILoggingInOutValueCollector<,>))
            //        .Where(type => !type.IsGenericType))
            //    .AsImplementedInterfaces()
            //    .WithTransientLifetime())
            .BuildServiceProvider();

        var result = await provider.GetService<IMediator>().Send(new SampleRequest() { Value = "Hello" });

        // Console.WriteLine(result.Value);

    }
}
