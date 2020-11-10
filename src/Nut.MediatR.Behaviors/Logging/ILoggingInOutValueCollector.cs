using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR
{
    public interface ILoggingInOutValueCollector<in TRequest, in TResponse>
    {
        Task<InOutValueResult> CollectInValueAsync(TRequest request, CancellationToken cancellationToken);

        Task<InOutValueResult> CollectOutValueAsync(TResponse response, CancellationToken cancellationToken);
    }
}