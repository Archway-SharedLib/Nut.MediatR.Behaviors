using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    public class RequestRegistry
    {
        private ConcurrentDictionary<string, MediatorRequest> requestPool = new ConcurrentDictionary<string, MediatorRequest>();

        public void Add(Type type, params Type[] filterTypes)
        {
            Add(type, false, filterTypes);
        }

        public void Add(Type type, bool ignoreDuplication, params Type[] filterTypes)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            FilterSupport.ThrowIfInvalidFileterTypeAllWith(filterTypes);

            var requests = MediatorRequest.Create(type, filterTypes);
            foreach (var request in requests)
            {
                if (!requestPool.TryAdd(request.Path, request))
                {
                    if(!ignoreDuplication)
                    {
                        throw new ArgumentException(SR.Registry_AlreadyContainsPath(request.Path), nameof(type));
                    }
                }
            }
        }

        public IEnumerable<string> GetEndpoints()
        {
            return requestPool.Keys;
        }

        public MediatorRequest? GetRequest(string path)
        {
            if(requestPool.TryGetValue(path, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
