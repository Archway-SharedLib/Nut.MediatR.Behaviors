using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ServiceFactory serviceFactory;

        public AuthorizationBehavior(ServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var authorizers = serviceFactory.GetInstances<IAuthorizer<TRequest>>();
            if (authorizers?.Any() == true)
            {
                foreach(var authorizer in authorizers)
                {
                    var result = await authorizer.AuthorizeAsync(request, cancellationToken).ConfigureAwait(false);
                    if (!result.Succeeded) throw new UnauthorizedException(
                        string.IsNullOrEmpty(result.FailurMessage) ? SR.Authorization_NotAuthorized : result.FailurMessage);
                }
            }
            
            return await next().ConfigureAwait(false);
        }
    }
}