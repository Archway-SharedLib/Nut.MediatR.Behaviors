using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Nut.MediatR.Test.Authorization;

public class AuthorizationBehaviorTest
{
    [Fact]
    public void ctor_引数がnullの場合は例外が発生する()
    {
        Action act = () => new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_Requestに一致する全てのAuthorizerが実行される()
    {
        var list = new List<string>();
        var factory = new ServiceFactory(type =>
        {
            return new IAuthorizer<TestBehaviorRequest>[]
            {
                    new SuccessAuthorizer1(list),
                    new SuccessAuthorizer2(list)
            };
        });
        var auth = new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(factory);
        await auth.Handle(new TestBehaviorRequest(), new CancellationToken(), () => Task.FromResult(new TestBehaviorResponse()));

        list.Count.Should().Be(2);
        list[0].Should().Be(AuthorizerMessages.SuccessAuthorizer1Message);
        list[1].Should().Be(AuthorizerMessages.SuccessAuthorizer2Message);
    }

    [Fact]
    public async Task Handle_RequestにするAuthorizerがnullの場合はなにも実行されない()
    {
        var list = new List<string>();
        var factory = new ServiceFactory(type =>
        {
            return null;
        });
        var auth = new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(factory);
        await auth.Handle(new TestBehaviorRequest(), new CancellationToken(), () => Task.FromResult(new TestBehaviorResponse()));

        list.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_RequestにするAuthorizerが空の場合はなにも実行されない()
    {
        var list = new List<string>();
        var factory = new ServiceFactory(type =>
        {
            return Enumerable.Empty<IAuthorizer<TestBehaviorRequest>>();
        });
        var auth = new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(factory);
        await auth.Handle(new TestBehaviorRequest(), new CancellationToken(), () => Task.FromResult(new TestBehaviorResponse()));

        list.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Authorizerが途中で失敗した場合はそこで処理が止まりUnauthorizedExceptionが発行される()
    {
        var list = new List<string>();
        var factory = new ServiceFactory(type =>
        {
            return new IAuthorizer<TestBehaviorRequest>[]
            {
                    new SuccessAuthorizer1(list),
                    new FailurAuthorizer1(list, "unauthorized!"),
                    new SuccessAuthorizer2(list)
            };
        });
        var auth = new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(factory);
        Func<Task> act = () => auth.Handle(new TestBehaviorRequest(), new CancellationToken(), () => Task.FromResult(new TestBehaviorResponse()));

        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("unauthorized!");

        list.Count.Should().Be(2);
        list[0].Should().Be(AuthorizerMessages.SuccessAuthorizer1Message);
        list[1].Should().Be(AuthorizerMessages.FailurAuthorizer1Message);
    }

    [Fact]
    public async Task Handle_失敗した時にメッセージが設定されていない場合は例外にデフォルトのメッセージが設定される()
    {
        using var cal = TestHelper.SetEnglishCulture();
        var list = new List<string>();
        var factory = new ServiceFactory(type =>
        {
            return new IAuthorizer<TestBehaviorRequest>[]
            {
                    new FailurAuthorizer1(list, string.Empty),
            };
        });
        var auth = new AuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(factory);
        Func<Task> act = () => auth.Handle(new TestBehaviorRequest(), new CancellationToken(), () => Task.FromResult(new TestBehaviorResponse()));

        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Not authorized.");

        list.Count.Should().Be(1);
        list[0].Should().Be(AuthorizerMessages.FailurAuthorizer1Message);
    }

    [Fact]
    public async Task Handle_GetAuthorizerからnullが返ったら処理が実行されずそのまま終了する()
    {
        var list = new List<string>();
        var factory = new ServiceFactory(type =>
        {
            return new IAuthorizer<TestBehaviorRequest>[]
            {
                    new SuccessAuthorizer1(list),
                    new FailurAuthorizer1(list, "unauthorized!"),
                    new SuccessAuthorizer2(list)
            };
        });
        var auth = new NullAuthorizationBehavior<TestBehaviorRequest, TestBehaviorResponse>(factory);
        await auth.Handle(new TestBehaviorRequest(), new CancellationToken(), () => Task.FromResult(new TestBehaviorResponse()));

        list.Should().BeEmpty();
    }
}

public class NullAuthorizationBehavior<TRequest, TResponse> : AuthorizationBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public NullAuthorizationBehavior(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    protected override IEnumerable<IAuthorizer<TRequest>> GetAuthorizers()
    {
        return null;
    }
}


public static class AuthorizerMessages
{
    public const string SuccessAuthorizer1Message = "SuccessAuthorizer1";
    public const string SuccessAuthorizer2Message = "SuccessAuthorizer2";
    public const string FailurAuthorizer1Message = "FailurAuthorizer1";
}

public class SuccessAuthorizer1 : IAuthorizer<TestBehaviorRequest>
{
    private readonly List<string> execHistory;

    public SuccessAuthorizer1(List<string> execHistory)
    {
        this.execHistory = execHistory;
    }

    public Task<AuthorizationResult> AuthorizeAsync(TestBehaviorRequest request, CancellationToken cancellationToken)
    {
        execHistory.Add(AuthorizerMessages.SuccessAuthorizer1Message);
        return Task.FromResult(AuthorizationResult.Success());
    }
}

public class SuccessAuthorizer2 : IAuthorizer<TestBehaviorRequest>
{
    private readonly List<string> execHistory;

    public SuccessAuthorizer2(List<string> execHistory)
    {
        this.execHistory = execHistory;
    }

    public Task<AuthorizationResult> AuthorizeAsync(TestBehaviorRequest request, CancellationToken cancellationToken)
    {
        execHistory.Add(AuthorizerMessages.SuccessAuthorizer2Message);
        return Task.FromResult(AuthorizationResult.Success());
    }
}

public class FailurAuthorizer1 : IAuthorizer<TestBehaviorRequest>
{
    private readonly List<string> execHistory;
    private readonly string message;

    public FailurAuthorizer1(List<string> execHistory, string message)
    {
        this.execHistory = execHistory;
        this.message = message;
    }

    public Task<AuthorizationResult> AuthorizeAsync(TestBehaviorRequest request, CancellationToken cancellationToken)
    {
        execHistory.Add(AuthorizerMessages.FailurAuthorizer1Message);
        return Task.FromResult(AuthorizationResult.Failed(message));
    }
}
