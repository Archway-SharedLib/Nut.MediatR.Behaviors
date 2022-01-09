using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR;

public interface ILoggingInOutValueCollector<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<InOutValueResult> CollectInValueAsync(TRequest request, CancellationToken cancellationToken);

    Task<InOutValueResult> CollectOutValueAsync(TResponse response, CancellationToken cancellationToken);
}
