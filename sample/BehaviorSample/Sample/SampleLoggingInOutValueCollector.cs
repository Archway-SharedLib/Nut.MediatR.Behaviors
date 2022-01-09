using System.Threading;
using System.Threading.Tasks;
using Nut.MediatR;

namespace BehaviorSample.Sample;

public class SampleLoggingInOutValueCollector : ILoggingInOutValueCollector<SampleRequest, SampleResponse>
{
    public Task<InOutValueResult> CollectInValueAsync(SampleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(request.Value));
    }

    public Task<InOutValueResult> CollectOutValueAsync(SampleResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(response.Value));
    }
}
