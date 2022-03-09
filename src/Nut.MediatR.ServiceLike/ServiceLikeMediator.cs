using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR.ServiceLike;

internal class ServiceLikeMediator : Mediator
{
    public ServiceLikeMediator(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    protected override async Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers, INotification notification, CancellationToken cancellationToken)
    {
        var exceptions = new List<Exception>();

        foreach (var handler in allHandlers)
        {
            try
            {
                await handler(notification, cancellationToken).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                exceptions.AddRange(ex.Flatten().InnerExceptions);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Any())
        {
            throw new AggregateException(exceptions);
        }
        // var tasks = allHandlers.Select(hanlder => hanlder(notification, cancellationToken));
        // return Task.WhenAll(tasks);
    }
}
