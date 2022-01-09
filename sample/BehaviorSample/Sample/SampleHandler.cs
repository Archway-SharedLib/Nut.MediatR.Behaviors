using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BehaviorSample.Sample;

public class SampleHandler : IRequestHandler<SampleRequest, SampleResponse>
{
    public Task<SampleResponse> Handle(SampleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SampleResponse() { Value = $"Response of {request.Value}" });
    }
}
