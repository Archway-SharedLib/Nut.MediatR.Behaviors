using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nut.MediatR
{
    public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ServiceFactory serviceFactory;

        public FluentValidationBehavior(ServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var validators = serviceFactory.GetInstances<IValidator<TRequest>>();
            if (validators?.Any() == true)
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToList();
                if (failures.Any()) throw new ValidationException(failures);
            }
            return await next().ConfigureAwait(false);
        }
    }
}
