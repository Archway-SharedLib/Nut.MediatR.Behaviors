using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Nut.MediatR.Test.Validation;

public class DataAnnotationValidationBehaviorTest
{
    [Fact]
    public async Task Handle_バリデーションエラーがない場合は何もおこらず完了する()
    {
        var executed = false;
        var behavior = new DataAnnotationValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>();
        await behavior.Handle(new TestBehaviorRequest()
        {
            Value = "A"
        }, () =>
        {
            executed = true;
            return Task.FromResult(new TestBehaviorResponse());
        }, new CancellationToken());
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_バリデーションエラーがある場合は例外が発生して処理が継続されない()
    {
        var executed = false;
        var behavior = new DataAnnotationValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>();
        var act = () => behavior.Handle(new TestBehaviorRequest()
        {
            Value = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        }, () =>
        {
            executed = true;
            return Task.FromResult(new TestBehaviorResponse());
        }, new CancellationToken());

        await act.Should().ThrowAsync<ValidationException>();
        executed.Should().BeFalse();
    }
}
