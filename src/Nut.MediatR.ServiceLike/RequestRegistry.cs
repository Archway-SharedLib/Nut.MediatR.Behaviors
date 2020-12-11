using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    public class RequestRegistry
    {
        private ConcurrentDictionary<string, MediatorRequest> requestPool = new ConcurrentDictionary<string, MediatorRequest>();

        public void Add(Type type)
        {
            Add(type, false);
        }

        public void Add(Type type, bool ignoreDuplication)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var requests = MediatorRequest.Create(type);
            foreach (var request in requests)
            {
                if (!requestPool.TryAdd(request.Path, request))
                {
                    if(!ignoreDuplication)
                    {
                        throw new ArgumentException($"Already contains path({request.Path})", nameof(type));
                    }
                }
            }
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
