using Nut.MediatR;
using Nut.MediatR.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorSample.Sample
{
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
}
