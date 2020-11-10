using System;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR
{
    public class InOutValueResult
    {
        private readonly object? value;

        private InOutValueResult(bool hasValue, object? value)
        {
            HasValue = hasValue;
            this.value = value;
        }

        public bool HasValue { get; }

        public object? Get()
        {
            if (!HasValue) throw new InvalidOperationException();
            return value;
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
}
