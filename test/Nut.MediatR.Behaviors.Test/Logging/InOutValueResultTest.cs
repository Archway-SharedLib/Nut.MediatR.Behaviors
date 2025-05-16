using System.Collections;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Nut.MediatR.Test.Logging;

public class InOutValueResultTest
{
    [Fact]
    public void Empty_値をもっていない結果が作成される()
    {
        InOutValueResult.Empty().HasValue.ShouldBeFalse();
    }

    [Fact]
    public void WithValue_値をもっている結果が作成される()
    {
        InOutValueResult.WithValue("A").HasValue.ShouldBeTrue();
    }

    [Fact]
    public void Get_値を取得できる()
    {
        InOutValueResult.WithValue("A").Get("value").ShouldBe("A");
    }

    [Fact]
    public void Get_対応するキーがない場合はnullが返る()
    {
        InOutValueResult.Empty().Get("value").ShouldBeNull();
    }

    [Fact]
    public void GetEnumerator_列挙できる()
    {
        var result = InOutValueResult.WithValue("A").Add("B", "B");
        var list = new List<KeyValuePair<string, object>>();
        foreach (KeyValuePair<string, object> item in (IEnumerable)result)
        {
            list.Add(item);
        }

        list.Count.ShouldBe(2);
        list[0].Key.ShouldBe("value");
        list[0].Value.ShouldBe("A");
        list[1].Key.ShouldBe("B");
        list[1].Value.ShouldBe("B");
    }
}
