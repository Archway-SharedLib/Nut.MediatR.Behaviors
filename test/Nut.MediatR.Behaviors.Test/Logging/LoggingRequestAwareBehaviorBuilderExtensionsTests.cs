using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.Behaviors.Tests;

public class LoggingRequestAwareBehaviorBuilderExtensionsTests
{
    [Fact]
    public void AddLogging_LoggingBehaviorとLoggingInOutValueCollectorが登録される()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);
        builder.AddLogging()
            .AddAssembliesForAutoRegister(typeof(ValueCollectorForTest).Assembly)
            .Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<LoggingBehavior<string, int>>().Should().NotBeNull();

        var collector = provider.GetService<ILoggingInOutValueCollector<string, int>>();
        collector.Should().NotBeNull();
        collector.Should().BeOfType<ValueCollectorForTest>();
    }

    [Fact]
    public void AddLogging_LoggingBehaviorと指定したアセンブリに含まれるLoggingInOutValueCollectorが登録される()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);
        var assemblies = new[] { typeof(ValueCollectorForTest).Assembly };

        builder.AddLogging(assemblies).Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<LoggingBehavior<string, int>>().Should().NotBeNull();

        var collector = provider.GetService<ILoggingInOutValueCollector<string, int>>();
        collector.Should().NotBeNull();
        collector.Should().BeOfType<ValueCollectorForTest>();
    }


    [Fact]
    public void AddLogging_アセンブリを指定していないときはLoggingInOutValueCollectorが登録されない()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);

        builder.AddLogging(null).Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<LoggingBehavior<string, int>>().Should().NotBeNull();

        var authorizer = provider.GetService<ILoggingInOutValueCollector<string, int>>();
        authorizer.Should().BeNull();
    }

    public class ValueCollectorForTest : ILoggingInOutValueCollector<string, int>
    {
    }
}
