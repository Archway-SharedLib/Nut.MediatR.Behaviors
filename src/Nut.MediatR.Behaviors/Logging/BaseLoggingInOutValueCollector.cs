using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR.Logging;

public abstract class BaseLoggingInOutValueCollector<TRequest, TResponse> : ILoggingInOutValueCollector<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public virtual Task<InOutValueResult> CollectInValueAsync(TRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.Empty());
    }

    public virtual Task<InOutValueResult> CollectOutValueAsync(TResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.Empty());
    }
}
