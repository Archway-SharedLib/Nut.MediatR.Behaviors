using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Nut.MediatR.Test.Logging;

public class InterfaceLoggingInOutValueCollectorTest
{
    [Fact]
    public async Task CollectInvalueAsync_デフォルト実装ではEmptyが返る()
    {
        ILoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse> target = new TestInterfaceLoggingInOutValueCollector();
        var result = await target.CollectInValueAsync(null, new System.Threading.CancellationToken());
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task CollectOutValueAsync_デフォルト実装ではEmptyが返る()
    {
        ILoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse> target = new TestInterfaceLoggingInOutValueCollector();
        var result = await target.CollectOutValueAsync(null, new System.Threading.CancellationToken());
        result.HasValue.ShouldBeFalse();
    }

    public class TestInterfaceLoggingInOutValueCollector :
        ILoggingInOutValueCollector<TestBehaviorRequest, TestBehaviorResponse>
    {
    }
}
