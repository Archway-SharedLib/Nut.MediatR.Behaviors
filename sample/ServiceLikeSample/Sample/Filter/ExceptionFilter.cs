using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nut.MediatR.ServiceLike;

namespace ServiceLikeSample.Sample.Filter;

public class ExceptionFilter : IMediatorServiceFilter
{
    public async Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
    {
        try
        {
            return await next(parameter);
        }
        catch (Exception ex)
        {
            var logger = context.ServiceFactory.GetInstance<ILogger<ExceptionFilter>>();
            logger.LogError(ex, "Error");
            throw ex;
        }
    }
}
