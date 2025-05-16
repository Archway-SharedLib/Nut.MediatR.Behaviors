using System;
using Shouldly;
using Microsoft.Extensions.DependencyInjection;
using Nut.MediatR.Internals;
using Xunit;
using System.Linq;

namespace Nut.MediatR.Test.Internals;
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
        Should.Throw<ArgumentNullException>(act);
    }

    [Fact]
    public void GetFirstServiceOrDefault_ServiceProviderがnullの場合は例外が発生する()
    {
        IServiceProvider serviceProvider = null;
        var act = () => serviceProvider.GetFirstServiceOrDefault<ITestService>();
        Should.Throw<ArgumentNullException>(act);
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
