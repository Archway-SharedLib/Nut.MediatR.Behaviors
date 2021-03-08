using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SR = Nut.MediatR.Resources.Strings;

namespace Nut.MediatR
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        protected ServiceFactory ServiceFactory { get; }

        public AuthorizationBehavior(ServiceFactory serviceFactory)
        {
            this.ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var authorizers = this.GetAuthorizers()?.ToList();
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

        protected virtual IEnumerable<IAuthorizer<TRequest>> GetAuthorizers()
        {
            return this.GetRegisterdAuthorizers();
        }

        /// <summary>
        /// Get regsiterd authorizers from <see cref="ServiceFactory"/>
        /// </summary>
        /// <returns>Regsiterd authorizers</returns>
        protected IEnumerable<IAuthorizer<TRequest>> GetRegisterdAuthorizers()
        {
            return ServiceFactory.GetInstances<IAuthorizer<TRequest>>() ?? Enumerable.Empty<IAuthorizer<TRequest>>();
        }
    }
}