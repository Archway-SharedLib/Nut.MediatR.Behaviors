using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Nut.MediatR.Test.Authorization;

public class AuthorizationBehaviorTest
{
    [Fact]
    public void ctor_引数がnullの場合は例外が発生する()
    {
        var act = () => new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        Should.Throw<ArgumentNullException>(act);
    }

    //---------------------------------

    [Fact]
    public async Task Handle_Requestに一致する全てのAuthorizerが実行される()
    {
        var list = new List<string>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(AuthorizationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddTransient<IAuthorizer<AuthorizeAllRequest>>(_ => new AuthorizeAllAuthorizer1(list));
        collection.AddTransient<IAuthorizer<AuthorizeAllRequest>>(_ => new AuthorizeAllAuthorizer2(list));
        collection.AddTransient<IAuthorizer<AuthorizeAllDummyRequest>>(_ => new AuthorizeAllDymmyAuthorizer(list));

        var provider = collection.BuildServiceProvider();
        await provider.GetService<IMediator>().Send(new AuthorizeAllRequest());

        list.Count.ShouldBe(2);
        list[0].ShouldBe("AuthorizeAllAuthorizer1");
        list[1].ShouldBe("AuthorizeAllAuthorizer2");
    }

    public record AuthorizeAllRequest : IRequest<AuthorizeAllResponse>;

    public record AuthorizeAllResponse;

    public class AuthorizeAllRequestHandler : IRequestHandler<AuthorizeAllRequest, AuthorizeAllResponse>
    {
        public Task<AuthorizeAllResponse> Handle(AuthorizeAllRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AuthorizeAllResponse());
        }
    }

    public record AuthorizeAllAuthorizer1(List<string> run) : IAuthorizer<AuthorizeAllRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(AuthorizeAllRequest request, CancellationToken cancellationToken)
        {
            run.Add("AuthorizeAllAuthorizer1");
            return Task.FromResult(AuthorizationResult.Success());
        }
    }

    public record AuthorizeAllAuthorizer2(List<string> run) : IAuthorizer<AuthorizeAllRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(AuthorizeAllRequest request, CancellationToken cancellationToken)
        {
            run.Add("AuthorizeAllAuthorizer2");
            return Task.FromResult(AuthorizationResult.Success());
        }
    }

    public record AuthorizeAllDummyRequest : IRequest<AuthorizeAllDummyResponse>;
    public record AuthorizeAllDummyResponse;

    public record AuthorizeAllDymmyAuthorizer(List<string> run) : IAuthorizer<AuthorizeAllDummyRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(AuthorizeAllDummyRequest request, CancellationToken cancellationToken)
        {
            run.Add("AuthorizeAllAuthorizer3");
            return Task.FromResult(AuthorizationResult.Success());
        }
    }

    //--------------

    [Fact]
    public async Task Handle_RequestするAuthorizerがnullの場合はなにも実行されない()
    {
        var list = new List<string>();
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEnumerable<IAuthorizer<TestBehaviorRequest>>)).Returns((IEnumerable<IAuthorizer<TestBehaviorRequest>>)null);
        var auth = new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(provider);
        await auth.Handle(new TestBehaviorRequest(), (_) => Task.FromResult(new TestBehaviorResponse()), CancellationToken.None);

        list.ShouldBeEmpty();
    }

    //---------

    [Fact]
    public async Task Handle_RequestにするAuthorizerが空の場合はなにも実行されない()
    {
        var list = new List<string>();
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEnumerable<IAuthorizer<TestBehaviorRequest>>)).Returns(Enumerable.Empty<IAuthorizer<TestBehaviorRequest>>());
        var auth = new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(provider);
        await auth.Handle(new TestBehaviorRequest(), (_) => Task.FromResult(new TestBehaviorResponse()), CancellationToken.None);

        list.ShouldBeEmpty();
    }

    //---------


    [Fact]
    public async Task Handle_Authorizerが途中で失敗した場合はそこで処理が止まりUnauthorizedExceptionが発行される()
    {
        var list = new List<string>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(AuthorizationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddTransient<IAuthorizer<AuthorizeFailRequest>>(_ => new AuthorizeFailAuthorizer1(list));
        collection.AddTransient<IAuthorizer<AuthorizeFailRequest>>(_ => new AuthorizeFailAuthorizer2(list));
        collection.AddTransient<IAuthorizer<AuthorizeFailRequest>>(_ => new AuthorizeFailAuthorizer3(list));

        var provider = collection.BuildServiceProvider();
        var act = () => provider.GetService<IMediator>().Send(new AuthorizeFailRequest());
        var ex = await Should.ThrowAsync<UnauthorizedException>(act);
        ex.Message.ShouldBe("Authorize Fail");

        list.Count.ShouldBe(2);
        list[0].ShouldBe("AuthorizeFailAuthorizer1");
        list[1].ShouldBe("AuthorizeFailAuthorizer2");
    }

    public record AuthorizeFailRequest : IRequest<AuthorizeFailResponse>;

    public record AuthorizeFailResponse;

    public class AuthorizeFailRequestHandler : IRequestHandler<AuthorizeFailRequest, AuthorizeFailResponse>
    {
        public Task<AuthorizeFailResponse> Handle(AuthorizeFailRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AuthorizeFailResponse());
        }
    }

    public record AuthorizeFailAuthorizer1(List<string> run) : IAuthorizer<AuthorizeFailRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(AuthorizeFailRequest request, CancellationToken cancellationToken)
        {
            run.Add("AuthorizeFailAuthorizer1");
            return Task.FromResult(AuthorizationResult.Success());
        }
    }

    public record AuthorizeFailAuthorizer2(List<string> run) : IAuthorizer<AuthorizeFailRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(AuthorizeFailRequest request, CancellationToken cancellationToken)
        {
            run.Add("AuthorizeFailAuthorizer2");
            return Task.FromResult(AuthorizationResult.Failed("Authorize Fail"));
        }
    }

    public record AuthorizeFailAuthorizer3(List<string> run) : IAuthorizer<AuthorizeFailRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(AuthorizeFailRequest request, CancellationToken cancellationToken)
        {
            run.Add("AuthorizeFailAuthorizer3");
            return Task.FromResult(AuthorizationResult.Success());
        }
    }

    //---------

    [Fact]
    public async Task Handle_失敗した時にメッセージが設定されていない場合は例外にデフォルトのメッセージが設定される()
    {
        using var _ = TestHelper.SetEnglishCulture();

        var list = new List<string>();
        var collection = new ServiceCollection();
        collection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(AuthorizationBehaviorTest).Assembly);
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        });
        collection.AddSingleton<Executed>();
        collection.AddTransient<IAuthorizer<AuthorizeFailDefaultMessageRequest>>(_ => new AuthorizeFailDefaultMessageAuthorizer1(list));

        var provider = collection.BuildServiceProvider();
        var act = () => provider.GetService<IMediator>().Send(new AuthorizeFailDefaultMessageRequest());
        var ex = await Should.ThrowAsync<UnauthorizedException>(act);
        ex.Message.ShouldBe("Not authorized.");
    }

    public record AuthorizeFailDefaultMessageRequest : IRequest<AuthorizeFailDefaultMessageResponse>;

    public record AuthorizeFailDefaultMessageResponse;

    public class AuthorizeFailDefaultMessageRequestHandler : IRequestHandler<AuthorizeFailDefaultMessageRequest, AuthorizeFailDefaultMessageResponse>
    {
        public Task<AuthorizeFailDefaultMessageResponse> Handle(AuthorizeFailDefaultMessageRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AuthorizeFailDefaultMessageResponse());
        }
    }

    public record AuthorizeFailDefaultMessageAuthorizer1(List<string> run) : IAuthorizer<AuthorizeFailDefaultMessageRequest>
    {
        public Task<AuthorizationResult> AuthorizeAsync(AuthorizeFailDefaultMessageRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(AuthorizationResult.Failed(null));
        }
    }
}
