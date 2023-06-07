using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Nut.MediatR.Behaviors.FluentValidation.Test;

public class FluentValidationBehaviorTest
{
    [Fact]
    public void ctor_ServiceFactory引数がnullの場合は例外が発生する()
    {
        Action act = () => new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_バリデータが空の場合はそのまま実行される()
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEnumerable<IValidator<TestBehaviorRequest>>)).Returns(new List<IValidator<TestBehaviorRequest>>());

        var executed = false;
        var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            provider);
        await behavior.Handle(
            new TestBehaviorRequest(),
            () =>
            {
                executed = true;
                return Task.FromResult(new TestBehaviorResponse());
            },
            new CancellationToken());
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_バリデータがnullの場合はそのまま実行される()
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEnumerable<IValidator<TestBehaviorRequest>>)).Returns((IEnumerable<IValidator<TestBehaviorRequest>>)null);

        var executed = false;
        var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            provider);
        await behavior.Handle(
            new TestBehaviorRequest(),
            () =>
            {
                executed = true;
                return Task.FromResult(new TestBehaviorResponse());
            },
            new CancellationToken());
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_バリデーションが実行されてエラーがある場合は例外が発生する()
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEnumerable<IValidator<TestBehaviorRequest>>)).Returns(new List<IValidator<TestBehaviorRequest>>() { new Validator1() });

        var executed = false;
        var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            provider);
        Func<Task> act = () => behavior.Handle(
            new TestBehaviorRequest(),
            () =>
            {
                executed = true;
                return Task.FromResult(new TestBehaviorResponse());
            }, new CancellationToken());

        var result = await act.Should().ThrowAsync<ValidationException>();
        result.And.Errors.Should().HaveCount(1);
        executed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_バリデーションが実行されてエラーがない場合は処理が継続する()
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEnumerable<IValidator<TestBehaviorRequest>>)).Returns(new List<IValidator<TestBehaviorRequest>>() { new Validator1() });

        var executed = false;
        var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            provider);
        await behavior.Handle(
            new TestBehaviorRequest() { Name = "A" },
            () =>
            {
                executed = true;
                return Task.FromResult(new TestBehaviorResponse());
            }, new CancellationToken());
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_複数バリデーションがある場合は全て実行されてエラーがある場合はマージされる()
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEnumerable<IValidator<TestBehaviorRequest>>)).Returns(new List<IValidator<TestBehaviorRequest>>() { new Validator1(), new Validator2() });

        var executed = false;
        var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
            provider);
        Func<Task> act = () => behavior.Handle(
            new TestBehaviorRequest(),
            () =>
            {
                executed = true;
                return Task.FromResult(new TestBehaviorResponse());
            }, new CancellationToken());

        var errors = await act.Should().ThrowAsync<ValidationException>();
        errors.And.Errors.Should().HaveCount(2);
        executed.Should().BeFalse();
    }
}

public class Validator1 : AbstractValidator<TestBehaviorRequest>
{
    public Validator1()
    {
        RuleFor(x => x.Name).NotNull();
    }
}

public class Validator2 : AbstractValidator<TestBehaviorRequest>
{
    public Validator2()
    {
        RuleFor(x => x.Age).GreaterThanOrEqualTo(1);
    }
}
