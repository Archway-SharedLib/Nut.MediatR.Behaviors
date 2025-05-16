using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
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
        }, (_) =>
        {
            executed = true;
            return Task.FromResult(new TestBehaviorResponse());
        }, CancellationToken.None);
        executed.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_バリデーションエラーがある場合は例外が発生して処理が継続されない()
    {
        var executed = false;
        var behavior = new DataAnnotationValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>();
        var act = () => behavior.Handle(new TestBehaviorRequest()
        {
            Value = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        }, (_) =>
        {
            executed = true;
            return Task.FromResult(new TestBehaviorResponse());
        }, CancellationToken.None);

        await Should.ThrowAsync<ValidationException>(act);
        executed.ShouldBeFalse();
    }
}
