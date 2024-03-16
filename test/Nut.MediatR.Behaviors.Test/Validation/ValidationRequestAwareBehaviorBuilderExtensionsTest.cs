using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.Test.Validation;

public class ValidationRequestAwareBehaviorBuilderExtensionsTest
{
    [Fact]
    public void AddValidation_DataAnnotationValidationBehaviorが登録される()
    {
        var serviceCollection = new ServiceCollection();
        var builder = new RequestAwareBehaviorBuilder(serviceCollection);

        builder.AddDataAnnotationValidation()
            .Build();
        var provider = serviceCollection.BuildServiceProvider();
        provider.GetService<DataAnnotationValidationBehavior<string, int>>().Should().NotBeNull();
    }
}
