using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR.ServiceLike;

public interface IMediatorServiceFilter
{
    Task<object> HandleAsync(RequestContext context, object? parameter, Func<object?, Task<object?>> next);
}
