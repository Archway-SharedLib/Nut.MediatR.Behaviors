using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test;

public class MediatorListenerDescriptionTest
{
    [Fact]
    public void Create_引数がnullの場合は例外が発行される()
    {
        Action act = () => MediatorListenerDescription.Create(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_引数の型のGenericがオープンしてたら例外が発行される()
    {
        Action act = () => MediatorListenerDescription.Create(typeof(OpenGenericPang<>));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_引数の型が実装型じゃない場合例外が発行される()
    {
        Action act = () => MediatorListenerDescription.Create(typeof(AbstractPang));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_引数の型がINotificationを継承していない場合例外が発行される()
    {
        Action act = () => MediatorListenerDescription.Create(typeof(PlainPang));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_引数の型にAsEventListenerが付加されていない場合例外が発行される()
    {
        Action act = () => MediatorListenerDescription.Create(typeof(OnlyPang));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_引数の型にINotificationを実装したクローズドでAsEventListenerが付加されている場合はMediatorListenerが返される()
    {
        var listeners = MediatorListenerDescription.Create(typeof(Pang));
        listeners.Should().HaveCount(1);
        var listener = listeners.First();
        listener.Key.Should().Be("pang");
        listener.ListenerType.Should().Be(typeof(Pang));
    }

    [Fact]
    public void Create_引数の型にINotificationを実装している場合はMediateTypeがNotificationのMediatorListenerが返される()
    {
        var listeners = MediatorListenerDescription.Create(typeof(Pang));
        listeners.Should().HaveCount(1);
        listeners.Select(l => l.MediateType)
            .All(m => m == MediateType.Notification).Should().BeTrue();
    }

    [Fact]
    public void Create_引数の型にIRequestを実装したクローズドでAsEventListenerが付加されている場合はMediatorListenerが返される()
    {
        var listeners = MediatorListenerDescription.Create(typeof(RequestPang));
        listeners.Should().HaveCount(1);
        var listener = listeners.First();
        listener.Key.Should().Be("pang.request");
        listener.ListenerType.Should().Be(typeof(RequestPang));
    }

    [Fact]
    public void Create_引数の型にIRequestTを実装したクローズドでAsEventListenerが付加されている場合はMediatorListenerが返される()
    {
        var listeners = MediatorListenerDescription.Create(typeof(RequestTPang));
        listeners.Should().HaveCount(1);
        var listener = listeners.First();
        listener.Key.Should().Be("pang.requestT");
        listener.ListenerType.Should().Be(typeof(RequestTPang));
    }

    [Fact]
    public void Create_引数の型にIRequestを実装している場合はMediateTypeがRequestのMediatorListenerが返される()
    {
        var listeners = MediatorListenerDescription.Create(typeof(RequestPang));
        listeners.Should().HaveCount(1);
        listeners.Select(l => l.MediateType)
            .All(m => m == MediateType.Request).Should().BeTrue();
    }

    [Fact]
    public void Create_引数の型にIRequestTを実装している場合はMediateTypeがRequestのMediatorListenerが返される()
    {
        var listeners = MediatorListenerDescription.Create(typeof(RequestTPang));
        listeners.Should().HaveCount(1);
        listeners.Select(l => l.MediateType)
            .All(m => m == MediateType.Request).Should().BeTrue();
    }

    [Fact]
    public void Create_引数の型に複数のAsEventListenerが付加されている場合は複数のEventが返される()
    {
        var listeners = MediatorListenerDescription.Create(typeof(MultiPang));
        listeners.Should().HaveCount(2);
        var listenerList = listeners.OrderBy(r => r.Key).ToList();
        listenerList[0].Key.Should().Be("pang.1");
        listenerList[0].ListenerType.Should().Be(typeof(MultiPang));
        listenerList[1].Key.Should().Be("pang.2");
        listenerList[1].ListenerType.Should().Be(typeof(MultiPang));
    }

}
