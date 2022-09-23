using System.Threading;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// 現在の <see cref="IServiceLikeContext"/> へのアクセスを提供します。
/// </summary>
public class ServiceLikeContextAccessor : IServiceLikeContextAccessor
{
    private static readonly AsyncLocal<ServiceLikeContextHolder> s_asyncLocalHolder = new();

    /// <summary>
    /// 現在の <see cref="IServiceLikeContext"/> を取得または設定します。現在の値がない場合は <see langword="null"/> が帰ります。
    /// </summary>
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
