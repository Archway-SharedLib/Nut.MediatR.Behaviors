using System.Reflection;
using FluentAssertions;
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
        stringService.Should().BeOfType<ServiceA>();
        var intService = serviceProvider.GetService<IService<int>>();
        intService.Should().BeOfType<ServiceB>();
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
