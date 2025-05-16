using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Nut.MediatR.Behaviors.FluentValidation.Test;

public class ServiceProviderExtensionsTest
{
    [Fact]
    public void GetServicesOrEmpty_登録されていない場合は空が返る()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var result = serviceProvider.GetServicesOrEmpty<ITestService>();
        result.ShouldBeEmpty();
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
        result.ShouldContain(x => x.GetType() == typeof(TestService));
        result.ShouldContain(x => x.GetType() == typeof(TestService2));
        result.ShouldContain(x => x.GetType() == typeof(TestService3));
        result.Count().ShouldBe(3);
    }

    [Fact]
    public void GetServicesOrEmpty_ServiceProviderがnullの場合は例外が発生する()
    {
        IServiceProvider serviceProvider = null;
        var act = () => serviceProvider.GetServicesOrEmpty<ITestService>();
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void GetServicesOrEmpty_GetServiceがnullの場合は空が返る()
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(Arg.Any<Type>()).Returns(null);
        var result = provider.GetServicesOrEmpty<ITestService>();
        result.ShouldBeEmpty();
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
