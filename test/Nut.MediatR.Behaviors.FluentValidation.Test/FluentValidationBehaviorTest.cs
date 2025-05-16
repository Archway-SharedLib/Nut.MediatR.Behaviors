using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Nut.MediatR.Behaviors.FluentValidation.Test;

public class FluentValidationBehaviorTest
{
    [Fact]
    public void ctor_ServiceFactory引数がnullの場合は例外が発生する()
    {
        Action act = () => new FluentValidationBehavior<NoValidatorRequest, NoValidatorResponse>(null);
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_バリデータが空の場合はそのまま実行される()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(FluentValidationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
        });
        collection.AddSingleton<Executed>();

        var provider = collection.BuildServiceProvider();

        var mediator = provider.GetService<IMediator>();
        var result = await mediator.Send(new NoValidatorRequest());

        var exec = provider.GetService<Executed>();
        exec.Value.ShouldBeTrue();
    }

    public record NoValidatorRequest : IRequest<NoValidatorResponse>;
    public record NoValidatorResponse;

    public class NoValidatorHandler(Executed executed) : IRequestHandler<NoValidatorRequest, NoValidatorResponse>
    {
        public Task<NoValidatorResponse> Handle(NoValidatorRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new NoValidatorResponse());
        }
    }

    [Fact]
    public async Task Handle_バリデーションが実行されてエラーがある場合は例外が発生する()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(FluentValidationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddValidatorsFromAssemblies([typeof(FluentValidationBehaviorTest).Assembly]);

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var act = () => mediator.Send(new ErrorValidatorRequest(null));
        var result = await act.ShouldThrowAsync<ValidationException>();
        result.Errors.Count().ShouldBe(1);

        var exec = provider.GetService<Executed>();
        exec.Value.ShouldBeFalse();
    }

    public record ErrorValidatorRequest(string Name) : IRequest<ErrorValidatorResponse>;
    public record ErrorValidatorResponse;

    public class ErrorValidatorHandler(Executed executed) : IRequestHandler<ErrorValidatorRequest, ErrorValidatorResponse>
    {
        public Task<ErrorValidatorResponse> Handle(ErrorValidatorRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new ErrorValidatorResponse());
        }
    }

    public class ErrorValidator : AbstractValidator<ErrorValidatorRequest>
    {
        public ErrorValidator()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }

    [Fact]
    public async Task Handle_バリデーションが実行されてエラーがない場合は処理が継続する()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(FluentValidationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddValidatorsFromAssemblies([typeof(FluentValidationBehaviorTest).Assembly]);

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var _ = await mediator.Send(new ErrorValidatorRequest("no error"));

        var exec = provider.GetService<Executed>();
        exec.Value.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_複数バリデーションがある場合は全て実行されてエラーがある場合はマージされる()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(FluentValidationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddValidatorsFromAssemblies([typeof(FluentValidationBehaviorTest).Assembly]);

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var act = () => mediator.Send(new MultipleErrorValidatorRequest(null, 0));
        var result = await act.ShouldThrowAsync<ValidationException>();
        result.Errors.Count().ShouldBe(2);

        var exec = provider.GetService<Executed>();
        exec.Value.ShouldBeFalse();
    }

    public record MultipleErrorValidatorRequest(string Name, int Age) : IRequest<MultipleErrorValidatorResponse>;
    public record MultipleErrorValidatorResponse;

    public class MultipleErrorValidatorHandler(Executed executed) : IRequestHandler<MultipleErrorValidatorRequest, MultipleErrorValidatorResponse>
    {
        public Task<MultipleErrorValidatorResponse> Handle(MultipleErrorValidatorRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.FromResult(new MultipleErrorValidatorResponse());
        }
    }

    public class MultipleErrorValidator1 : AbstractValidator<MultipleErrorValidatorRequest>
    {
        public MultipleErrorValidator1()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }

    public class MultipleErrorValidator2 : AbstractValidator<MultipleErrorValidatorRequest>
    {
        public MultipleErrorValidator2()
        {
            RuleFor(x => x.Age).GreaterThanOrEqualTo(1);
        }
    }

    [Fact]
    public async Task Handle_Void_バリデーションが実行されてエラーがある場合は例外が発生する()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(FluentValidationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddValidatorsFromAssemblies([typeof(FluentValidationBehaviorTest).Assembly]);

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        var act = () => mediator.Send(new VoidErrorValidatorRequest(null));
        var result = await act.ShouldThrowAsync<ValidationException>();
        result.Errors.Count().ShouldBe(1);

        var exec = provider.GetService<Executed>();
        exec.Value.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_Void_バリデーションが実行されてエラーが無い場合は処理がかんりょうする()
    {
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(FluentValidationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddValidatorsFromAssemblies([typeof(FluentValidationBehaviorTest).Assembly]);

        var provider = collection.BuildServiceProvider();
        var mediator = provider.GetService<IMediator>();
        await mediator.Send(new VoidErrorValidatorRequest("no error"));

        var exec = provider.GetService<Executed>();
        exec.Value.ShouldBeTrue();
    }

    public record VoidErrorValidatorRequest(string Name) : IRequest;

    public class VoidErrorValidatorHandler(Executed executed) : IRequestHandler<VoidErrorValidatorRequest>
    {
        public Task Handle(VoidErrorValidatorRequest request, CancellationToken cancellationToken)
        {
            executed.Value = true;
            return Task.CompletedTask;
        }
    }

    public class VoidErrorValidator : AbstractValidator<VoidErrorValidatorRequest>
    {
        public VoidErrorValidator()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }
}

public class Executed
{
    public bool Value { get; set; }
}

