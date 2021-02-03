using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLikeSample.Sample.Notification
{
    public class handler1 : INotificationHandler<SampleEvent>
    {
        private readonly ILogger<handler1> logger;

        public handler1(ILogger<handler1> logger)
        {
            this.logger = logger;
        }

        public Task Handle(SampleEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"handler1 {notification}");
            return Task.CompletedTask;
        }
    }

    public class handler2 : INotificationHandler<SampleEvent>
    {
        private readonly ILogger<handler2> logger;

        public handler2(ILogger<handler2> logger)
        {
            this.logger = logger;
        }
        public Task Handle(SampleEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"handler2 {notification}");
            return Task.CompletedTask;
        }
    }

    public class handler3 : INotificationHandler<SampleEvent>
    {
        private readonly ILogger<handler3> logger;

        public handler3(ILogger<handler3> logger)
        {
            this.logger = logger;
        }
        public Task Handle(SampleEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"handler3 {notification}");
            return Task.CompletedTask;
        }
    }
}
