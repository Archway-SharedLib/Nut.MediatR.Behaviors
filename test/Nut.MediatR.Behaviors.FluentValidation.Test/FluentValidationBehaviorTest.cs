using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nut.MediatR.Behaviors.FluentValidation.Test
{
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
            var executed = false;
            var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
                new ServiceFactory(_ => new List<IValidator<TestBehaviorRequest>>()));
            await behavior.Handle(
                new TestBehaviorRequest(),
                new CancellationToken(),
                () =>
                {
                    executed = true;
                    return Task.FromResult(new TestBehaviorResponse());
                });
            executed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_バリデータがnullの場合はそのまま実行される()
        {
            var executed = false;
            var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
                new ServiceFactory(_ => null));
            await behavior.Handle(
                new TestBehaviorRequest(),
                new CancellationToken(),
                () =>
                {
                    executed = true;
                    return Task.FromResult(new TestBehaviorResponse());
                });
            executed.Should().BeTrue();
        }

        [Fact]
        public void Handle_バリデーションが実行されてエラーがある場合は例外が発生する()
        {
            var executed = false;
            var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
                new ServiceFactory(_ => new List<IValidator<TestBehaviorRequest>>() { new Validator1() }));
            Func<Task> act = () => behavior.Handle(
                new TestBehaviorRequest(),
                new CancellationToken(),
                () =>
                {
                    executed = true;
                    return Task.FromResult(new TestBehaviorResponse());
                });

            act.Should().Throw<ValidationException>().And.Errors.Should().HaveCount(1);
            executed.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_バリデーションが実行されてエラーがない場合は処理が継続する()
        {
            var executed = false;
            var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
                new ServiceFactory(_ => new List<IValidator<TestBehaviorRequest>>() { new Validator1() }));
            await behavior.Handle(
                new TestBehaviorRequest() { Name = "A" },
                new CancellationToken(),
                () =>
                {
                    executed = true;
                    return Task.FromResult(new TestBehaviorResponse());
                });
            executed.Should().BeTrue();
        }

        [Fact]
        public void Handle_複数バリデーションがある場合は全て実行されてエラーがある場合はマージされる()
        {
            var executed = false;
            var behavior = new FluentValidationBehavior<TestBehaviorRequest, TestBehaviorResponse>(
                new ServiceFactory(_ => new List<IValidator<TestBehaviorRequest>>() { new Validator1(), new Validator2() }));
            Func<Task> act = () => behavior.Handle(
                new TestBehaviorRequest(),
                new CancellationToken(),
                () =>
                {
                    executed = true;
                    return Task.FromResult(new TestBehaviorResponse());
                });

            var errors = act.Should().Throw<ValidationException>().And.Errors.Should().HaveCount(2);
            executed.Should().BeFalse();
        }
    }

    public class Validator1 : AbstractValidator<TestBehaviorRequest>
    {
        public Validator1()
        {
            this.RuleFor(x => x.Name).NotNull();
        }
    }

    public class Validator2 : AbstractValidator<TestBehaviorRequest>
    {
        public Validator2()
        {
            this.RuleFor(x => x.Age).GreaterThanOrEqualTo(1);
        }
    }

}
