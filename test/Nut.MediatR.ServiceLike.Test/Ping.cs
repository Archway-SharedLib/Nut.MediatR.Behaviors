using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike.Test
{
    [AsService("/ping")]
    public class ServicePing: IRequest<Pong>
    {
        public string Value { get; set; }
    }

    [AsService("/ping")]
    public class ServicePing2 : IRequest<Pong>
    {
        public string Value { get; set; }
    }

    [AsService("/ping/1")]
    [AsService("/ping/2")]
    public class MultiServicePing : IRequest<Pong>
    {
        public string Value { get; set; }
    }

    [AsService("/ping/null")]
    public class ServiceNullPing : IRequest<Pong>
    {
        public string Value { get; set; }
    }

    [AsService("/ping/void")]
    public class VoidServicePing : IRequest { }

    public class NonServicePing: IRequest<Pong>
    {
    }

    public class VoidNonServicePing : IRequest
    {
    }

    public class Pong
    {
        public string Value { get; set; }
    }

    public class ServicePingHandler : IRequestHandler<ServicePing, Pong>
    {
        public Task<Pong> Handle(ServicePing request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong() { Value = request.Value + " Pong" });
        }
    }

    public class ServiceNullPingHandler : IRequestHandler<ServiceNullPing, Pong>
    {
        public Task<Pong> Handle(ServiceNullPing request, CancellationToken cancellationToken)
        {
            return Task.FromResult(null as Pong);
        }
    }

    public class VoidServicePingHandler : IRequestHandler<VoidServicePing>
    {
        public Task<Unit> Handle(VoidServicePing request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }


}
