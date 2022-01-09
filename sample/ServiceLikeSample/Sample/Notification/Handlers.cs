using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nut.MediatR.ServiceLike;

namespace ServiceLikeSample.Sample.Notification
{
    public class handler1 : INotificationHandler<SampleEvent>
    {
        private readonly ILogger<handler1> logger;
        private readonly IServiceLikeContextAccessor serviceLikeContextAccessor;
        private readonly IServiceProvider serviceProvider;

        public handler1(ILogger<handler1> logger, IServiceLikeContextAccessor serviceLikeContextAccessor, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceLikeContextAccessor = serviceLikeContextAccessor;
            this.serviceProvider = serviceProvider;
        }

        public Task Handle(SampleEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"handler1 {notification}");

            var context = serviceLikeContextAccessor.Context;
            logger.LogInformation($"handler 1 {context.Id} {context.Timestamp}");

            Task.Run(() =>
            {
                var s = serviceProvider.GetService(typeof(IServiceLikeContextAccessor)) as IServiceLikeContextAccessor;
                var context2 = s.Context;
                logger.LogInformation($"handler 1 ---  {context2.Id} {context2.Timestamp}");
            });

            return Task.CompletedTask;
        }
    }

    public class handler2 : INotificationHandler<SampleEvent>
    {
        private readonly ILogger<handler2> logger;
        private readonly IServiceLikeContextAccessor serviceLikeContextAccessor;

        public handler2(ILogger<handler2> logger, IServiceLikeContextAccessor serviceLikeContextAccessor)
        {
            this.logger = logger;
            this.serviceLikeContextAccessor = serviceLikeContextAccessor;
        }
        public Task Handle(SampleEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"handler2 {notification}");

            var context = serviceLikeContextAccessor.Context;
            logger.LogInformation($"handler 2 {context.Id} {context.Timestamp}");
            return Task.CompletedTask;
        }
    }

    public class handler3 : INotificationHandler<SampleEvent>
    {
        private readonly ILogger<handler3> logger;
        private readonly IServiceLikeContextAccessor serviceLikeContextAccessor;

        public handler3(ILogger<handler3> logger, IServiceLikeContextAccessor serviceLikeContextAccessor)
        {
            this.logger = logger;
            this.serviceLikeContextAccessor = serviceLikeContextAccessor;
        }
        public Task Handle(SampleEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"handler3 {notification}");

            var context = serviceLikeContextAccessor.Context;
            logger.LogInformation($"handler 3 {context.Id} {context.Timestamp}");
            return Task.CompletedTask;
        }
    }
}
