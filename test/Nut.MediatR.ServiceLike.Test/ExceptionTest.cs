using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test;

public class ExceptionTest
{
    // for coverage
    [Fact]
    public void MediatRServiceLikeException_ctor()
    {
        var exception = new MediatRServiceLikeException();
        exception.Should().NotBeNull();
    }

    [Fact]
    public void MediatRServiceLikeException_メッセージが設定される()
    {
        var message = "testmessage";
        var exception = new MediatRServiceLikeException(message);
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void RequestNotFoundException_RequestPathが設定される()
    {
        var requestPath = "/path";
        var exception = new ReceiverNotFoundException(requestPath);
        exception.RequestPath.Should().Be(requestPath);
    }

    [Fact]
    public void RequestNotFoundException_メッセージはデフォルトの値が設定される()
    {
        var requestPath = "/path";
        var exception = new ReceiverNotFoundException(requestPath);
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void RequestNotFoundException_メッセージが設定される()
    {
        var requestPath = "/path";
        var message = "testmessage";
        var exception = new ReceiverNotFoundException(requestPath, message);
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void TypeTranslationException_FromToが設定される()
    {
        var from = typeof(string);
        var to = typeof(int);
        var exception = new TypeTranslationException(from, to);
        exception.FromType.Should().Be(from);
        exception.ToType.Should().Be(to);
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void TypeTranslationException_メッセージが設定される()
    {
        var from = typeof(string);
        var to = typeof(int);
        var message = "testmessage";
        var exception = new TypeTranslationException(from, to, message);
        exception.FromType.Should().Be(from);
        exception.ToType.Should().Be(to);
        exception.Message.Should().Be(message);
    }

}
