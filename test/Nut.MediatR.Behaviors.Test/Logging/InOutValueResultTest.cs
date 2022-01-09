using System;
using System.Collections.Generic;
using System.Text;
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
        InOutValueResult.WithValue("A").Get().Should().Be("A");
    }

    [Fact]
    public void Get_Emptyの場合は例外が発生する()
    {
        Action act = () => InOutValueResult.Empty().Get();
        act.Should().Throw<InvalidOperationException>();
    }
}
