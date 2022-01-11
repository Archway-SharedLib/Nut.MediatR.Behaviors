using System.Threading;

namespace Nut.MediatR.ServiceLike;

public class ServiceLikeContextAccessor : IServiceLikeContextAccessor
{
    private static readonly AsyncLocal<ServiceLikeContextHolder> s_asyncLocalHolder = new();

    public IServiceLikeContext? Context
    {
        get => s_asyncLocalHolder.Value?.Context;
        set
        {
            var holder = s_asyncLocalHolder.Value;
            if (holder != null)
            {
                // Clear current HttpContext trapped in the AsyncLocals, as its done.
                holder.Context = null;
            }

            if (value != null)
            {
                s_asyncLocalHolder.Value = new ServiceLikeContextHolder { Context = value };
            }
        }
    }

    private class ServiceLikeContextHolder
    {
        public IServiceLikeContext? Context;
    }
}
