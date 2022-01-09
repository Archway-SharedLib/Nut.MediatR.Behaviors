using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ServiceLikeSample.Sample.Filter
{
    public class FilterHandler : IRequestHandler<FilterRequest, FilterResult>
    {
        private readonly ILogger<FilterHandler> logger;

        public FilterHandler(ILogger<FilterHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<FilterResult> Handle(FilterRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation(request.Id);
            return Task.FromResult(new FilterResult() { Name = "Test" });
        }
    }
}
