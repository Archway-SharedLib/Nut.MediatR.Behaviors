using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR.ServiceLike
{
    public class MediatorRequest
    {
        private MediatorRequest(string path, Type requestType, IEnumerable<Type> filters)
        {
            Path = path;
            RequestType = requestType;
            Filters = filters;
        }

        public static IEnumerable<MediatorRequest> Create(Type requestType, params Type[] filterTypes)
        {
            if (requestType is null)
            {
                throw new ArgumentNullException(nameof(requestType));
            }
            FilterSupport.ThrowIfInvalidFileterTypeAllWith(filterTypes);

            if (!CanServicalize(requestType))
            {
                throw new ArgumentException("The requestType argument specify implement IRequest(<T>) and the closed generic type with AsAserviceAttribute.",
                    nameof(requestType));
            }
            var attrs = requestType.GetAttributes<AsServiceAttribute>(true);
            return attrs.Select(attr => 
            {
                var attrFilters = attr.FilterTypes;
                var filters = filterTypes.Concat(attrFilters).ToList();
                return new MediatorRequest(attr.Path, requestType, filters);
            }).ToList();
        }

        public Type RequestType { get; }

        public IEnumerable<Type> Filters { get; }
        
        public string Path { get; }

        public static bool CanServicalize(Type requestType)
            => !requestType.IsOpenGeneric()
                && requestType.IsConcrete()
                && (requestType.IsImplemented(typeof(IRequest<>)) 
                    || requestType.IsImplemented(typeof(IRequest)))
                && requestType.GetAttributes<AsServiceAttribute>().Any();
    }
}