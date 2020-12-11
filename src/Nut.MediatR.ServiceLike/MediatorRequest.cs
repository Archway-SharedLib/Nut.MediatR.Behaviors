using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR.ServiceLike
{
    public class MediatorRequest
    {
        private MediatorRequest(string path, Type requestType)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
        }

        public static IEnumerable<MediatorRequest> Create(Type requestType)
        {
            if (requestType.IsOpenGeneric() || !requestType.IsImplemented(typeof(IRequest<>)))
            {
                throw new ArgumentException("The requestType argument specify implement IRequest<> and the closed generic type.", nameof(requestType));
            }
            var attrs = requestType.GetAttributes<AsServiceAttribute>();
            if (attrs is null || !attrs.Any())
            {
                throw new ArgumentException("The requestType must have AsServiceAttribute", nameof(requestType));
            }
            return attrs.Select(attr => new MediatorRequest(attr.Path, requestType)).ToList();
        }

        public Type RequestType { get; }

        public string Path { get; }
    }
}