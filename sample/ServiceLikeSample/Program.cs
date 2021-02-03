using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Nut.MediatR.ServiceLike.DependencyInjection;
using Nut.MediatR.ServiceLike;
using System.Threading.Tasks;
using ServiceLikeSample.Sample.Basic;
using System.Collections.Generic;
using ServiceLikeSample.Sample;
using ServiceLikeSample.ServiceDto;
using ServiceLikeSample.Sample.Filter;

namespace ServiceLikeSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = new ServiceCollection()
                .AddLogging(config =>
                {
                    config.AddConsole();
                })
                .AddMediatR(typeof(Program))
                .AddMediatRServiceLike(typeof(Program).Assembly, typeof(ExceptionFilter))
                .BuildServiceProvider();

            var mediator = provider.GetService<IMediator>();
            var logger = provider.GetService<ILogger<Program>>();
            var client = provider.GetService<IMediatorClient>();

            // 基本
            var result1 = await client.SendAsync<Output>("/basic", new Input("123"));
            logger.LogInformation(result1.Name);

            // 匿名型で送る
            var result2 = await client.SendAsync<Output>("/basic", new { Id = "345" });
            logger.LogInformation(result2.Name);

            // ディクショナリで送る
            var result3 = await client.SendAsync<Output>("/basic", new Dictionary<string, object>() { { "Id", "678" } });
            logger.LogInformation(result3.Name);

            // ディクショナリで受け取る
            var result4 = await client.SendAsync<Dictionary<string, object>>("/basic", new Dictionary<string, object>() { { "Id", "901" } });
            logger.LogInformation(result4["Name"].ToString());

            // Filterを使う
            var result5 = await client.SendAsync<Output>("/filter", new { Id = "345" });
            logger.LogInformation(result5.Name);

            // Eventを使う
            await client.PublishAsync("Mediator.SampleEvent", new { Id = "123", Name = "Bob", Age = 23 });
            logger.LogInformation("Complete Event Publish");

        }
    }
}
