using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nut.MediatR
{
    public class DataAnnotationValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public DataAnnotationValidationBehavior()
        {
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            Validator.ValidateObject(request, new ValidationContext(request), true);
            return await next().ConfigureAwait(false);
        }
    }
}
