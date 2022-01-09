using System.Threading;
using System.Threading.Tasks;
using Nut.MediatR;

namespace BehaviorSample.Sample;

public class SampleAuthorizer : IAuthorizer<SampleRequest>
{
    public Task<AuthorizationResult> AuthorizeAsync(SampleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(AuthorizationResult.Success());
    }
}
