using System.Threading;
using System.Threading.Tasks;

namespace Nut.MediatR;

public interface IAuthorizer<TRequest>
{
    public Task<AuthorizationResult> AuthorizeAsync(TRequest request, CancellationToken cancellationToken);
}
