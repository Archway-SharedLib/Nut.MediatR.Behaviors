using System.Reflection;
using Shouldly;
using Microsoft.Extensions.DependencyInjection;
using Nut.MediatR.Internals;
using Xunit;

namespace Nut.MediatR.Test.Internals;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void TryAddTransientGenericInterfaceTypeFromAssemblies_ShouldRegisterImplementations()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var assemblies = new[] { Assembly.GetExecutingAssembly() };
        var serviceType = typeof(IService<>);

        // Act
        serviceCollection.TryAddTransientGenericInterfaceTypeFromAssemblies(assemblies, serviceType);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var stringService = serviceProvider.GetService<IService<string>>();
        stringService.ShouldBeOfType<ServiceA>();
        var intService = serviceProvider.GetService<IService<int>>();
        intService.ShouldBeOfType<ServiceB>();
    }
}

public interface IService<T>
{
}

public class ServiceA : IService<string>
{
}

public class ServiceB : IService<int>
{
}
