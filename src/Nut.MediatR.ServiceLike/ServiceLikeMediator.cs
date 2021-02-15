using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike
{
    internal class ServiceLikeMediator : Mediator
    {
        public ServiceLikeMediator(ServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        protected override Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers, INotification notification, CancellationToken cancellationToken)
        {
            var tasks = allHandlers.Select(hanlder => hanlder(notification, cancellationToken));
            return Task.WhenAll(tasks);
        }
    }
}
