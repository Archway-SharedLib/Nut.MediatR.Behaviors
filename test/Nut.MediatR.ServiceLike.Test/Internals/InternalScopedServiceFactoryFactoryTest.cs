using System;
using FluentAssertions;
using Nut.MediatR.ServiceLike.Internals;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test.Internals
{
    public class InternalScopedServiceFactoryFactoryTest
    {
        [Fact]
        public void ctor_引数にnullを設定すると例外が発生する()
        {
            Action act = () => new InternalScopedServiceFactoryFactory(null!);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}