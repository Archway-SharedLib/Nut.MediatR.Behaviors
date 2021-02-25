using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Nut.MediatR.ServiceLike.DependencyInjection.Test
{
    public class ScopedServiceFactoryFactoryTest
    {
        [Fact]
        public void ctor_パラメーターがnullの場合は例外が発生する()
        {
            Action act = () => new ScopedServiceFactoryFactory(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_Scopeが返ってくる()
        {
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();
            var factory = new ScopedServiceFactoryFactory(provider.GetService<IServiceScopeFactory>());
            factory.Create().Should().NotBeNull();
        }
    }
}