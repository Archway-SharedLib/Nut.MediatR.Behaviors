using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// サービスのレジストリです。
/// </summary>
public class ServiceRegistry
{
    private readonly ConcurrentDictionary<string, MediatorServiceDescription> _servicePool = new();

    /// <summary>
    /// サービスを追加します。
    /// </summary>
    /// <param name="type">サービスの <see cref="Type"/></param>
    /// <param name="filterTypes">フィルターの型</param>
    public void Add(Type type, params Type[] filterTypes)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        Add(type, false, filterTypes);
    }

    /// <summary>
    /// サービスを追加します。
    /// </summary>
    /// <param name="type">サービスの <see cref="Type"/></param>
    /// <param name="ignoreDuplication">同じサービスが登録されたときにエラーにするかどうか</param>
    /// <param name="filterTypes">フィルターの型</param>
    public void Add(Type type, bool ignoreDuplication, params Type[] filterTypes)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        FilterSupport.ThrowIfInvalidFilterTypeAllWith(filterTypes);

        var services = MediatorServiceDescription.Create(type, filterTypes);
        foreach (var service in services)
        {
            if (!_servicePool.TryAdd(service.Path, service))
            {
                if (!ignoreDuplication)
                {
                    throw new ArgumentException(SR.Registry_AlreadyContainsPath(service.Path), nameof(type));
                }
            }
        }
    }

    /// <summary>
    /// 登録されているサービスのキーを取得します。
    /// </summary>
    /// <returns>登録されているサービスのキー</returns>
    [Obsolete("Please use GetKeys().", false)]
    public IEnumerable<string> GetEndpoints() => _servicePool.Keys;

    /// <summary>
    /// 登録されているサービスのキーを取得します。
    /// </summary>
    /// <returns>登録されているサービスのキー</returns>
    public IEnumerable<string> GetKeys() => _servicePool.Keys;

    /// <summary>
    /// キーを指定してサービスを取得します。
    /// </summary>
    /// <param name="key">取得するサービスのキー</param>
    /// <returns>指定されたキーで登録されているサービス</returns>
    public MediatorServiceDescription? GetService(string key)
        => _servicePool.TryGetValue(key, out var value) ? value : null;
}
