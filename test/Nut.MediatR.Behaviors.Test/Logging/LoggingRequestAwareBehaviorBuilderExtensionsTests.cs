using Shouldly;
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
        provider.GetService<LoggingBehavior<string, int>>().ShouldNotBeNull();

        var collector = provider.GetService<ILoggingInOutValueCollector<string, int>>();
        collector.ShouldNotBeNull();
        collector.ShouldBeOfType<ValueCollectorForTest>();
    }

    [Fact]
    public void AddLogging_LoggingBehaviorと指定したアセンブリに含まれるLoggingInOutValueCollectorが登録される()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);
        var assemblies = new[] { typeof(ValueCollectorForTest).Assembly };

        builder.AddLogging(assemblies).Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<LoggingBehavior<string, int>>().ShouldNotBeNull();

        var collector = provider.GetService<ILoggingInOutValueCollector<string, int>>();
        collector.ShouldNotBeNull();
        collector.ShouldBeOfType<ValueCollectorForTest>();
    }


    [Fact]
    public void AddLogging_アセンブリを指定していないときはLoggingInOutValueCollectorが登録されない()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);

        builder.AddLogging(null).Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<LoggingBehavior<string, int>>().ShouldNotBeNull();

        var authorizer = provider.GetService<ILoggingInOutValueCollector<string, int>>();
        authorizer.ShouldBeNull();
    }

    public class ValueCollectorForTest : ILoggingInOutValueCollector<string, int>
    {
    }
}
