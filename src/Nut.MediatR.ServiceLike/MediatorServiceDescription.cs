using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

public class MediatorServiceDescription
{
    private MediatorServiceDescription(string path, Type serviceType, IEnumerable<Type> filters)
    {
        Path = path;
        ServiceType = serviceType;
        Filters = filters;
    }

    public static IEnumerable<MediatorServiceDescription> Create(Type serviceType, params Type[] filterTypes)
    {
        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }
        FilterSupport.ThrowIfInvalidFilterTypeAllWith(filterTypes);

        if (!CanServicalize(serviceType))
        {
            throw new ArgumentException(SR.Argument_CanNotServicalize(nameof(serviceType)));
        }
        var attrs = serviceType.GetAttributes<AsServiceAttribute>(true);
        return attrs.Select(attr =>
        {
            var attrFilters = attr.FilterTypes;
            var filters = filterTypes.Concat(attrFilters).ToList();
            return new MediatorServiceDescription(attr.Path, serviceType, filters);
        }).ToList();
    }

    public Type ServiceType { get; }

    public IEnumerable<Type> Filters { get; }

    public string Path { get; }

    public static bool CanServicalize(Type requestType)
        => !requestType.IsOpenGeneric()
            && requestType.IsConcrete()
            && (requestType.IsImplemented(typeof(IRequest<>))
                || requestType.IsImplemented(typeof(IRequest)))
            && requestType.GetAttributes<AsServiceAttribute>().Any();
}
