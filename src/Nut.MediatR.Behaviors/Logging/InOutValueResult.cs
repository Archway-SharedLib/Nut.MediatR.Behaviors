using System;

namespace Nut.MediatR;

public class InOutValueResult
{
    private readonly object? _value;

    private InOutValueResult(bool hasValue, object? value)
    {
        HasValue = hasValue;
        _value = value;
    }

    public bool HasValue { get; }

    public object? Get()
    {
        if (!HasValue) throw new InvalidOperationException();
        return _value;
    }

    public static InOutValueResult Empty()
    {
        return new InOutValueResult(false, null);
    }

    public static InOutValueResult WithValue(object? value)
    {
        return new InOutValueResult(true, value);
    }
}
