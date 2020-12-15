using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class AsServiceAttributeTest
    {
        [Fact]
        public void ctor_パラメーターがnullの場合は例外が発生する()
        {
            Action act = () => new AsServiceAttribute(null);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_パラメーターが空文字の場合は例外が発生する()
        {
            Action act = () => new AsServiceAttribute("");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_パラメーターがホワイトスペースの場合は例外が発生する()
        {
            Action act = () => new AsServiceAttribute(" ");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_パラメーターで指定したPathが取得できる()
        {
            var expect = "/this/is/service/path";
            var attr = new AsServiceAttribute(expect);
            attr.Path.Should().Be(expect);
        }

        [Fact]
        public void ctor_filterTypesがnullの場合は例外が発生する()
        {
            Action act = () => new AsServiceAttribute("/this/is/service/path", null);
            act.Should().Throw<ArgumentException>();
        }
    }
}
