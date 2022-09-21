namespace Nut.MediatR.ServiceLike;

/// <summary>
/// <see cref="IServiceLikeContext"/> を取得するためのインターフェイスを定義します。
/// </summary>
public interface IServiceLikeContextAccessor
{
    /// <summary>
    /// <see cref="IServiceLikeContext"/> を取得または設定します。
    /// </summary>
    IServiceLikeContext? Context { get; set; }
}
