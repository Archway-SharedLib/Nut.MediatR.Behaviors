using System;
using System.Collections.Generic;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;
using System.Collections.ObjectModel;

namespace Nut.MediatR.ServiceLike
{
    public class ServiceLikeContext: IServiceLikeContext
    {
        public ServiceLikeContext(string key) : this(key, null) { }

        public ServiceLikeContext(string key, IDictionary<string, object>? header)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(key)));
            }

            Key = key;
            Header = new ReadOnlyDictionary<string, object>(header ?? new Dictionary<string, object>());
            Id = Guid.NewGuid().ToString("N");
            Timestamp = DateTimeOffset.Now.Ticks;
        }

        public string Id { get; }

        public string Key { get; }
        public IDictionary<string, object> Header { get; }

        public long Timestamp { get; }
    }
}
