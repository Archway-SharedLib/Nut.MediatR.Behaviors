using System;
using MediatR;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// スコープで限定された <see cref="ServiceFactory"/>
/// </summary>
public interface IScoepedServiceFactory : IDisposable
{
    /// <summary>
    /// インスタンスを取得します。
    /// </summary>
    ServiceFactory Instance { get; }
}
