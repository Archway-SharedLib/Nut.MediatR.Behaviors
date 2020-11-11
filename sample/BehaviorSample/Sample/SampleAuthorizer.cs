using Nut.MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorSample.Sample
{
    public class SampleAuthorizer : IAuthorizer<SampleRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(SampleRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(AuthorizationResult.Success());
        }
    }
}
