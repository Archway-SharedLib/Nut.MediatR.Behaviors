using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test;

public class ServiceLikeContextTest
{
    [Fact]
    public void ctor_keyにnullまたは空文字または空白が指定された場合は例外が発生する()
    {
        ((Action)(() => new ServiceLikeContext(null))).Should().Throw<ArgumentException>();
        ((Action)(() => new ServiceLikeContext(""))).Should().Throw<ArgumentException>();
        ((Action)(() => new ServiceLikeContext("  "))).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ctor_Headerにnullを設定してもからのHeaderがプロパティから取得できる()
    {
        var instance = new ServiceLikeContext("key", null);
        instance.Header.Count().Should().Be(0);
    }

    [Fact]
    public void Id_IdにはGuidベースの値が設定される()
    {
        var instance = new ServiceLikeContext("key");
        Guid.TryParseExact(instance.Id, "N", out var _).Should().BeTrue();
    }

    [Fact]
    public void Header_コンストラクタで指定した内容が設定されている()
    {
        var header = new Dictionary<string, object>()
            {
                { "key1", "123" },
                { "key2", 456 },
            };
        var instance = new ServiceLikeContext("key", header);

        instance.Header.Count().Should().Be(header.Count());
        instance.Header["key1"].Should().Be(header["key1"]);
        instance.Header["key2"].Should().Be(header["key2"]);
    }

    [Fact]
    public void Key_ctorで指定した値が設定される()
    {
        var expect = "alksjfolawjef";
        var instance = new ServiceLikeContext(expect);
        instance.Key.Should().Be(expect);
    }

    [Fact]
    public void Timestamp_現在日時が設定される()
    {
        var now = DateTimeOffset.Now.Ticks;
        var instance = new ServiceLikeContext("foo");
        instance.Timestamp.Should().BeInRange(now - 30000, now + 30000);
    }
}
