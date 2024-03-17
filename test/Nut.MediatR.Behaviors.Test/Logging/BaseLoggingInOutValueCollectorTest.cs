using System.Threading.Tasks;
using FluentAssertions;
using Nut.MediatR.Logging;
using Xunit;

namespace Nut.MediatR.Test.Logging;

public class BaseLoggingInOutValueCollectorTest
{
    [Fact]
    public async Task CollectInValueAsync_デフォルトはEmpty()
    {
        var target = new TestBaseLoggingInOutValueCollectorExt();
        var result = await target.CollectInValueAsync(null, new System.Threading.CancellationToken());
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task CollectInOutValueAsync_デフォルトはEmpty()
    {
        var target = new TestBaseLoggingInOutValueCollectorExt();
        var result = await target.CollectOutValueAsync(null, new System.Threading.CancellationToken());
        result.HasValue.Should().BeFalse();
    }
}

public class TestBaseLoggingInOutValueCollectorExt :
    BaseLoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse>
{

}
