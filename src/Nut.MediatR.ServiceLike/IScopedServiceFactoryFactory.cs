namespace Nut.MediatR.ServiceLike;

/// <summary>
/// <see cref="IScoepedServiceFactory"/> のインスタンスを取得するためのインターフェイスを定義します。
/// </summary>
public interface IScopedServiceFactoryFactory
{
    /// <summary>
    /// <see cref="IScoepedServiceFactory"/> のインスタンスを生成します。
    /// </summary>
    /// <returns><see cref="IScoepedServiceFactory"/> のインスタンス</returns>
    IScoepedServiceFactory Create();
}
