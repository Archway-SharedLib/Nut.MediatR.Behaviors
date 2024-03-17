using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nut.MediatR.Internals;
using Xunit;

namespace Nut.MediatR.Test.Internals;
public class ServiceProviderExtensionsTest
{
    [Fact]
    public void GetServicesOrEmpty_登録されていない場合は空が返る()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var result = serviceProvider.GetServicesOrEmpty<ITestService>();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetServicesOrEmpty_登録されている場合は全てが返る()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<ITestService, TestService>();
        collection.AddTransient<ITestService, TestService2>();
        collection.AddScoped<ITestService, TestService3>();

        var serviceProvider = collection.BuildServiceProvider();

        var result = serviceProvider.GetServicesOrEmpty<ITestService>();
        result.Should().Contain(x => x.GetType() == typeof(TestService));
        result.Should().Contain(x => x.GetType() == typeof(TestService2));
        result.Should().Contain(x => x.GetType() == typeof(TestService3));
        result.Should().HaveCount(3);
    }

    [Fact]
    public void GetServicesOrEmpty_ServiceProviderがnullの場合は例外が発生する()
    {
        IServiceProvider serviceProvider = null;
        var act = () => serviceProvider.GetServicesOrEmpty<ITestService>();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetFirstServiceOrDefault_ServiceProviderがnullの場合は例外が発生する()
    {
        IServiceProvider serviceProvider = null;
        var act = () => serviceProvider.GetFirstServiceOrDefault<ITestService>();
        act.Should().Throw<ArgumentNullException>();
    }

    public interface ITestService
    {
    }

    public class TestService : ITestService
    {
    }
    public class TestService2 : ITestService
    {
    }
    public class TestService3 : ITestService
    {
    }
}
