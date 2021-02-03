using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class AsEventAttributeTest
    {
        [Fact]
        public void ctor_パラメーターがnullの場合は例外が発生する()
        {
            Action act = () => new AsEventAttribute(null);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_パラメーターが空文字の場合は例外が発生する()
        {
            Action act = () => new AsEventAttribute("");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_パラメーターがホワイトスペースの場合は例外が発生する()
        {
            Action act = () => new AsEventAttribute(" ");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_パラメーターで指定したPathが取得できる()
        {
            var expect = "/this/is/service/path";
            var attr = new AsEventAttribute(expect);
            attr.Path.Should().Be(expect);
        }
    }
}
