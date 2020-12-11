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
            Path = path;
            RequestType = requestType;
        }

        public static IEnumerable<MediatorRequest> Create(Type requestType)
        {
            if (requestType is null)
            {
                throw new ArgumentNullException(nameof(requestType));
            }

            if (!CanServicalize(requestType))
            {
                throw new ArgumentException("The requestType argument specify implement IRequest(<T>) and the closed generic type with AsAserviceAttribute.", 
                    nameof(requestType));
            }
            var attrs = requestType.GetAttributes<AsServiceAttribute>(true);
            return attrs.Select(attr => new MediatorRequest(attr.Path, requestType)).ToList();
        }

        public Type RequestType { get; }

        public string Path { get; }

        public static bool CanServicalize(Type requestType)
            => !requestType.IsOpenGeneric()
                && requestType.IsConcrete()
                && (requestType.IsImplemented(typeof(IRequest<>)) 
                    || requestType.IsImplemented(typeof(IRequest)))
                && requestType.GetAttributes<AsServiceAttribute>().Any();
    }
}