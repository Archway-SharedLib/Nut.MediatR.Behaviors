using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLikeSample.Sample.Void
{
    public class VoidHandler : IRequestHandler<VoidRequest, Unit>
    {
        private readonly ILogger<VoidHandler> logger;

        public VoidHandler(ILogger<VoidHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<Unit> Handle(VoidRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation(request.Id);
            return Task.FromResult(Unit.Value);
        }
    }
}
