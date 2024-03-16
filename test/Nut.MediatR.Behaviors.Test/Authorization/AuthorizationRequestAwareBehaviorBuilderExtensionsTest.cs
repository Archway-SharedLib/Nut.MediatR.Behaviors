using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.Test.Authorization;

public class AuthorizationRequestAwareBehaviorBuilderExtensionsTest
{
    [Fact]
    public void AddAuthorization_AuthorizationBehaviorとAuthorizerが登録される()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);

        builder.AddAuthorization()
            .AddAssembliesForAutoRegister(typeof(AuthorizerForTest).Assembly)
            .Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<AuthorizationBehavior<string, int>>().Should().NotBeNull();

        var authorizer = provider.GetService<IAuthorizer<string>>();
        authorizer.Should().NotBeNull();
        authorizer.Should().BeOfType<AuthorizerForTest>();
    }

    public class AuthorizerForTest : IAuthorizer<string>
    {
        public Task<AuthorizationResult> AuthorizeAsync(string request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    [Fact]
    public void AddAuthorization_AuthorizationBehaviorと指定したアセンブリに含まれるAuthorizerが登録される()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);
        var assemblies = new[] { typeof(AuthorizationRequestAwareBehaviorBuilderExtensionsTest).Assembly };

        builder.AddAuthorization(assemblies).Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<AuthorizationBehavior<string, int>>().Should().NotBeNull();

        var authorizer = provider.GetService<IAuthorizer<string>>();
        authorizer.Should().NotBeNull();
        authorizer.Should().BeOfType<AuthorizerForTest>();
    }


    [Fact]
    public void AddAuthorization_アセンブリを指定していないときはAuthorizerが登録されない()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);

        builder.AddAuthorization(null).Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<AuthorizationBehavior<string, int>>().Should().NotBeNull();

        var authorizer = provider.GetService<IAuthorizer<string>>();
        authorizer.Should().BeNull();
    }
}
