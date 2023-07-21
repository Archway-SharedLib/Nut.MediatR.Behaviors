using System;
using MediatR;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// スコープで限定された <see cref="IServiceProvider"/>
/// </summary>
public interface IScoepedServiceFactory : IDisposable
{
    /// <summary>
    /// インスタンスを取得します。
    /// </summary>
    IServiceProvider Instance { get; }
}
