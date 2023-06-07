using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR.ServiceLike;

internal class ServiceLikeMediator : Mediator
{
    public ServiceLikeMediator(IServiceProvider serviceFactory) : base(serviceFactory)
    {
    }

    protected virtual async Task PublishCore(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        var exceptions = new List<Exception>();

        foreach (var handler in handlerExecutors)
        {
            try
            {
                await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
                // await handler(notification, cancellationToken).ConfigureAwait(false);
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


    //protected override async Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers, INotification notification, CancellationToken cancellationToken)
    //{
    //    var exceptions = new List<Exception>();

    //    foreach (var handler in allHandlers)
    //    {
    //        try
    //        {
    //            await handler(notification, cancellationToken).ConfigureAwait(false);
    //        }
    //        catch (AggregateException ex)
    //        {
    //            exceptions.AddRange(ex.Flatten().InnerExceptions);
    //        }
    //        catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
    //        {
    //            exceptions.Add(ex);
    //        }
    //    }

    //    if (exceptions.Any())
    //    {
    //        throw new AggregateException(exceptions);
    //    }
    //    // var tasks = allHandlers.Select(hanlder => hanlder(notification, cancellationToken));
    //    // return Task.WhenAll(tasks);
    //}
}
