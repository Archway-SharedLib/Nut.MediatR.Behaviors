using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike
{
    public interface IFilter
    {
        Task<object> HandleAsync(RequestContext context, object? parameter, Func<object?, Task<object?>> next);
    }
}
