using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// 各Publisherで発生した例外を集約して投げるPublisher
/// </summary>
public class ForeachAllAwaitPublisher : INotificationPublisher
{
    /// <summary>
    /// 指定された処理を発行します。
    /// </summary>
    /// <param name="handlerExecutors">ハンドラを実行する <see cref="NotificationHandlerExecutor"/> の列挙</param>
    /// <param name="notification">通知する <see cref="INotification"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>非同期の実行結果</returns>
    /// <exception cref="AggregateException">各ハンドラで発生した例外を集約した例外</exception>
    public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        var exceptions = new List<Exception>();

        foreach (var handler in handlerExecutors)
        {
            try
            {
                await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                exceptions.AddRange(ex.Flatten().InnerExceptions);
            }
            catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Any())
        {
            throw new AggregateException(exceptions);
        }
    }
}
