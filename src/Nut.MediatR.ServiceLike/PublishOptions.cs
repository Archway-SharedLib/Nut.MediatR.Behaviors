using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike
{
    public class PublishOptions
    {
        public IDictionary<string, object> Header { get; } = new Dictionary<string, object>();

        public Func<object, IServiceLikeContext, Task>? BeforePublishAsyncHandler { get; set; }

        public Func<object, IServiceLikeContext, Task>? CompleteAsyncHandler { get; set; }

        public Func<Exception, object, IServiceLikeContext, Task>? ErrorAsyncHandler { get; set; }
    }
}
