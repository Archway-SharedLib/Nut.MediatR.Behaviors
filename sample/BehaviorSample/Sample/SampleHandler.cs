﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorSample.Sample
{
    public class SampleHandler : IRequestHandler<SampleRequest, SampleResponse>
    {
        public Task<SampleResponse> Handle(SampleRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SampleResponse() { Value = $"Response of {request.Value}" });
        }
    }
}
