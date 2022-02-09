using System;

namespace Nut.MediatR;

/// <summary>
/// <see cref="ILoggingInOutValueCollector{TRequest,TResponse}"/> の処理結果を表すオブジェクトです。
/// </summary>
public class InOutValueResult
{
    private readonly object? _value;

    private InOutValueResult(bool hasValue, object? value)
    {
        HasValue = hasValue;
        _value = value;
    }

    /// <summary>
    /// 値が設定されているかどうかを取得します。
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// 値を取得します。
    /// </summary>
    /// <returns><see cref="ILoggingInOutValueCollector{TRequest,TResponse}"/> の処理結果の値</returns>
    /// <exception cref="InvalidOperationException">値が設定されていない場合に発生します。</exception>
    public object? Get()
    {
        if (!HasValue) throw new InvalidOperationException();
        return _value;
    }

    /// <summary>
    /// 値無しの結果のインスタンスを返します。
    /// </summary>
    /// <returns>値無しの結果のインスタンス</returns>
    public static InOutValueResult Empty()
    {
        return new InOutValueResult(false, null);
    }

    /// <summary>
    /// 指定した値が設定された結果のインスタンスを返します。
    /// </summary>
    /// <param name="value">結果の値</param>
    /// <returns>指定した値が設定された結果のインスタンス</returns>
    public static InOutValueResult WithValue(object? value)
    {
        return new InOutValueResult(true, value);
    }
}
