using System;
using Shouldly;
using Xunit;

namespace Nut.MediatR.Test.RequestAware;

public class WithBehaviorsAttributeTest
{
    [Fact]
    public void ctor_パラメーターがnullの場合は例外が発生する()
    {
        Action act = () => new WithBehaviorsAttribute(null);
        Should.Throw<ArgumentNullException>(act);
    }

    [Fact]
    public void ctor_パラメーターの中にnullが含まれている場合は例外が発生する()
    {
        Action act = () => new WithBehaviorsAttribute(typeof(TestBehavior1<,>), null, typeof(TestBehavior2<,>));
        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void ctor_パラメーターの中にIPipelineBehavior以外の型が含まれている場合は例外が発生する()
    {
        Action act = () => new WithBehaviorsAttribute(typeof(TestBehavior1<,>), typeof(string), typeof(TestBehavior2<,>));
        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void BehaviorTypes_コンストラクタで設定された順にTypeが取得できる()
    {
        var attr = new WithBehaviorsAttribute(typeof(TestBehavior2<,>), typeof(TestBehavior1<,>));
        attr.BehaviorTypes.Count.ShouldBe(2);
        attr.BehaviorTypes.ShouldBe(new[] { typeof(TestBehavior2<,>), typeof(TestBehavior1<,>) });
    }
}
