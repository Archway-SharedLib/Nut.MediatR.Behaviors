using System;
using System.Collections.Generic;

namespace Nut.MediatR.ServiceLike
{
    public class PublishOptions
    {
        public IDictionary<string, object> Header { get; } = new Dictionary<string, object>();

        public Action<object, IServiceLikeContext>? CompleteHandler { get; set; }

        public Action<Exception, object, IServiceLikeContext>? ErrorHandler { get; set; }
    }
}
