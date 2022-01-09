using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nut.MediatR.ServiceLike;

namespace ServiceLikeSample.Sample.Filter
{
    public class ParamAndResultValueConverterFilter : IMediatorServiceFilter
    {
        public async Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
        {
            if (parameter is FilterRequest req)
            {
                req.Id += " Req";
            }
            var result = await next(parameter);
            if (result is FilterResult res)
            {
                res.Name += " Res";
            }
            return result;
        }
    }
}
