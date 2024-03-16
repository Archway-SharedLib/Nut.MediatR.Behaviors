using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Nut.MediatR.Test.Logging;

public class InOutValueResultTest
{
    [Fact]
    public void Empty_値をもっていない結果が作成される()
    {
        InOutValueResult.Empty().HasValue.Should().BeFalse();
    }

    [Fact]
    public void WithValue_値をもっている結果が作成される()
    {
        InOutValueResult.WithValue("A").HasValue.Should().BeTrue();
    }

    [Fact]
    public void Get_値を取得できる()
    {
        InOutValueResult.WithValue("A").Get("value").Should().Be("A");
    }

    [Fact]
    public void Get_対応するキーがない場合はnullが返る()
    {
        InOutValueResult.Empty().Get("value").Should().BeNull();
    }

    [Fact]
    public void GetEnumerator_列挙できる()
    {
        var result = InOutValueResult.WithValue("A").Add("B", "B");
        var list = new List<KeyValuePair<string, object?>>();
        foreach (KeyValuePair<string, object?> item in (IEnumerable)result)
        {
            list.Add(item);
        }

        list.Should().HaveCount(2);
        list[0].Key.Should().Be("value");
        list[0].Value.Should().Be("A");
        list[1].Key.Should().Be("B");
        list[1].Value.Should().Be("B");
    }
}
