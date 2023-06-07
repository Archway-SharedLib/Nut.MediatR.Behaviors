using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR;

/// <summary>
/// 処理の実装前後でログを出力します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// <see cref="ServiceProvider"/> を取得します。
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="serviceProvider">サービスを取得する <see cref="ServiceProvider"/></param>
    public LoggingBehavior(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// ログに出力する追加の値を取得するための <see cref="ILoggingInOutValueCollector{TRequest, TResponse}"/> の実装を取得します。
    /// </summary>
    /// <returns>ログに出力する追加の値を取得するための <see cref="ILoggingInOutValueCollector{TRequest, TResponse}"/> の実装</returns>
    protected virtual ILoggingInOutValueCollector<TRequest, TResponse>? GetDefaultCollector() => null;

    /// <summary>
    /// ログを出力します。
    /// </summary>
    /// <param name="request">リクエスト</param>
    /// <param name="next">次の処理</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>処理結果</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var logger = ServiceProvider.GetFirstServiceOrDefault<ILogger<LoggingBehavior<TRequest, TResponse>>>();
        if (logger is null)
        {
            return await next().ConfigureAwait(false);
        }

        var collector = ServiceProvider.GetFirstServiceOrDefault<ILoggingInOutValueCollector<TRequest, TResponse>>()
            ?? GetDefaultCollector();

        var inValue = collector is not null ?
            await collector.CollectInValueAsync(request, cancellationToken).ConfigureAwait(false) :
            null;

        if (inValue?.HasValue == true)
        {
            logger.Log(LogLevel.Information, "Start {Request}. {Input}", typeof(TRequest).Name, inValue.Get());
        }
        else
        {
            logger.Log(LogLevel.Information, "Start {Request}.", typeof(TRequest).Name);
        }

        var watch = new Stopwatch();
        watch.Start();
        try
        {
            var result = await next().ConfigureAwait(false);
            watch.Stop();

            var outValue = collector is not null ?
                await collector.CollectOutValueAsync(result, cancellationToken).ConfigureAwait(false) :
                null;

            if (outValue?.HasValue == true)
            {
                logger.Log(LogLevel.Information, "Complete {Request} in {Elapsed}ms. {Output}",
                    typeof(TRequest).Name,
                    watch.ElapsedMilliseconds,
                    outValue.Get());
            }
            else
            {
                logger.Log(LogLevel.Information, "Complete {Request} in {Elapsed}ms.",
                    typeof(TRequest).Name,
                    watch.ElapsedMilliseconds);
            }

            return result;
        }
        catch (Exception e)
        {
            watch.Stop();
            logger.Log(LogLevel.Error, e, "Exception {Request} in {Elapsed}ms.", typeof(TRequest).Name, watch.ElapsedMilliseconds, e.Message);
            throw;
        }
    }
}
