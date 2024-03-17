using System;
using System.Collections;
using System.Collections.Generic;

namespace Nut.MediatR;

/// <summary>
/// <see cref="ILoggingInOutValueCollector{TRequest,TResponse}"/> の処理結果を表すオブジェクトです。
/// </summary>
public class InOutValueResult : IEnumerable<KeyValuePair<string, object?>>
{
    private readonly Dictionary<string, object?> _values = new();
    private const string DefaultKey = "value";

    private InOutValueResult()
    {
    }

    private InOutValueResult(string key, object? value)
    {
        _values.Add(key, value);
    }

    /// <summary>
    /// 値が設定されているかどうかを取得します。
    /// </summary>
    public bool HasValue => _values.Count > 0;

    /// <summary>
    /// 値を取得します。
    /// </summary>
    /// <returns><see cref="ILoggingInOutValueCollector{TRequest,TResponse}"/> の処理結果の値</returns>
    /// <exception cref="InvalidOperationException">値が設定されていない場合に発生します。</exception>
    public object? Get(string key)
    {
        if (_values.TryGetValue(key, out var value))
            return value;
        return null;
    }

    /// <summary>
    /// 値を追加します。
    /// </summary>
    /// <param name="key">追加するキー</param>
    /// <param name="value">追加する値</param>
    /// <returns>このインスタンス</returns>
    public InOutValueResult Add(string key, object? value)
    {
        _values[key] = value;
        return this;
    }

    /// <summary>
    /// 値無しの結果のインスタンスを返します。
    /// </summary>
    /// <returns>値無しの結果のインスタンス</returns>
    public static InOutValueResult Empty()
    {
        return new InOutValueResult();
    }

    /// <summary>
    /// 指定した値が設定された結果のインスタンスを返します。
    /// </summary>
    /// <param name="value">結果の値</param>
    /// <returns>指定した値が設定された結果のインスタンス</returns>
    public static InOutValueResult WithValue(object? value)
    {
        return new InOutValueResult(DefaultKey, value);
    }

    /// <summary>
    /// 指定したキーと値が設定された結果のインスタンスを返します。
    /// </summary>
    /// <param name="key">結果のキー</param>
    /// <param name="value">結果の値</param>
    /// <returns>指定したキーと値が設定された結果のインスタンス</returns>
    public static InOutValueResult WithKeyValue(string key, object? value)
    {
        return new InOutValueResult(key, value);
    }

    /// <summary>
    /// <see cref="KeyValuePair{TKey, TValue}"/> の列挙子を取得します。
    /// </summary>
    /// <returns><see cref="KeyValuePair{TKey, TValue}"/> の列挙子</returns>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _values.GetEnumerator();

    /// <summary>
    /// 列挙しを取得します。
    /// </summary>
    /// <returns>列挙子</returns>
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
}
