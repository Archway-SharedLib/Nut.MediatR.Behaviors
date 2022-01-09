using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR;

public class DataAnnotationValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
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
