using System;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike;

public interface IMediatorServiceFilter
{
    Task<object> HandleAsync(RequestContext context, object? parameter, Func<object?, Task<object?>> next);
}
