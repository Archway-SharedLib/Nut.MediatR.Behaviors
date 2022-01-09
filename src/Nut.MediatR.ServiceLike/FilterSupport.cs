using System;
using System.Collections.Generic;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

internal class FilterSupport
{
    public static void ThrowIfInvalidFilterTypeAllWith(IEnumerable<Type> filterTypes)
    {
        var result = IsValidFilterTypeAllCore(filterTypes);
        if (!result)
        {
            throw new ArgumentException(SR.FilterTypeConstratins);
        }
    }

    public static bool IsValidFilterTypeAllCore(IEnumerable<Type> filterTypes)
    {
        if (filterTypes is null)
        {
            throw new ArgumentNullException(nameof(filterTypes));
        }

        foreach (var type in filterTypes)
        {
            if (type is null) return false;
            if (!IsFilterType(type)) return false;
        }
        return true;
    }

    private static bool IsFilterType(Type behaviorType)
        => behaviorType.IsImplemented(typeof(IMediatorServiceFilter)) && behaviorType.HasDefaultConstructor();
}
