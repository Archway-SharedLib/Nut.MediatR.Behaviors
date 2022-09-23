using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// リスナーのレジストリです。
/// </summary>
public class ListenerRegistry
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<MediatorListenerDescription>> _listenerPool = new();

    /// <summary>
    /// リスナーを追加します。
    /// </summary>
    /// <param name="type">リスナーの<see cref="Type"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> が <see langword="null"/> の場合に発生します。</exception>
    public void Add(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var listeners = MediatorListenerDescription.Create(type);
        foreach (var listener in listeners)
        {
            var bag = _listenerPool.GetOrAdd(listener.Key, key => new ConcurrentBag<MediatorListenerDescription>());
            bag.Add(listener);
        }
    }

    /// <summary>
    /// 登録されているリスナーのキー
    /// </summary>
    /// <returns>登録されているリスナーのキー</returns>
    public IEnumerable<string> GetKeys() => _listenerPool.Keys;

    /// <summary>
    /// キーを指定してリスナーを取得します。
    /// </summary>
    /// <param name="key">取得するリスナーのキー</param>
    /// <returns>指定されたキーで登録されているリスナー</returns>
    public IEnumerable<MediatorListenerDescription> GetListeners(string key)
        => _listenerPool.TryGetValue(key, out var value) ? value : Enumerable.Empty<MediatorListenerDescription>();
}
