using FluentAssertions;
using Nut.MediatR.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nut.MediatR.Test.Logging
{
    public class BaseLoggingInOutValueCollectorTest
    {
        [Fact]
        public async Task CollectInValueAsync_デフォルトはEmpty()
        {
            var target = new TestBaseLogginInOutVlaueCollectorExt();
            var result = await target.CollectInValueAsync(null, new System.Threading.CancellationToken());
            result.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task CollectInOutValueAsync_デフォルトはEmpty()
        {
            var target = new TestBaseLogginInOutVlaueCollectorExt();
            var result = await target.CollectOutValueAsync(null, new System.Threading.CancellationToken());
            result.HasValue.Should().BeFalse();
        }
    }

    public class TestBaseLogginInOutVlaueCollectorExt : 
        BaseLoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse>
    {

    }

}
