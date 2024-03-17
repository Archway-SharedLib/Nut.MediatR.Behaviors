using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nut.MediatR.Internals;

namespace Nut.MediatR;

/// <summary>
/// 処理の実装前後でログを出力します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private const string RequestKey = "Mediator.Request";
    private const string ElapsedKey = "Mediator.Elapsed";

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

        var collector = GetCollector() ?? GetDefaultCollector();

        var inValue = collector is not null ?
            await collector.CollectInValueAsync(request, cancellationToken).ConfigureAwait(false) :
            null;

        if (inValue?.HasValue == true)
        {
            OutputWithValues(logger, inValue, $"Start {{{RequestKey}}}. ", typeof(TRequest).Name);
        }
        else
        {
            logger.Log(LogLevel.Information, $"Start {{{RequestKey}}}.", typeof(TRequest).Name);
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
                OutputWithValues(logger, outValue, $"Complete {{{RequestKey}}} in {{{ElapsedKey}}}ms. ", typeof(TRequest).Name, watch.ElapsedMilliseconds);
            }
            else
            {
                logger.Log(LogLevel.Information, $"Complete {{{RequestKey}}} in {{{ElapsedKey}}}ms.",
                    typeof(TRequest).Name,
                    watch.ElapsedMilliseconds);
            }
            return result;
        }
        catch (Exception e)
        {
            watch.Stop();
            logger.Log(LogLevel.Error, e, $"Exception {{{RequestKey}}} in {{{ElapsedKey}}}ms.", typeof(TRequest).Name, watch.ElapsedMilliseconds, e.Message);
            throw;
        }
    }

    private static void OutputWithValues(ILogger<LoggingBehavior<TRequest, TResponse>> logger, InOutValueResult valueResult, string baseMessage, params object[] baseValues)
    {
        var sb = new StringBuilder(baseMessage);
        var values = new List<object?>(baseValues);
        var first = true;
        foreach (var (key, value) in valueResult)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append(", ");
            }
            sb.Append($"{key}: {{{key}}}");
            values.Add(value);
        }
        logger.Log(LogLevel.Information, sb.ToString(), values.ToArray());
    }

    private ILoggingInOutValueCollector<TRequest, TResponse>? GetCollector()
    {
        if (typeof(TResponse) == typeof(Unit))
        {
            var requestOnly = ServiceProvider.GetService<ILoggingInOutValueCollector<TRequest>>();
            if (requestOnly is not null)
            {
                return requestOnly as ILoggingInOutValueCollector<TRequest, TResponse>;
            }
        }
        return ServiceProvider.GetFirstServiceOrDefault<ILoggingInOutValueCollector<TRequest, TResponse>>();
    }
}
