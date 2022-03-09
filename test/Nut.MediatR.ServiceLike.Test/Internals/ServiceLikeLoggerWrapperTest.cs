using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nut.MediatR.ServiceLike.Internals;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test.Internals;

public class ServiceLikeLoggerWrapperTest
{
    [Fact]
    public void ctor_もとのロガーがnullでも例外は発生しない()
    {
        new ServiceLikeLoggerWrapper(null);
    }

    [Fact]
    public void ErrorOnPublish_元のロガーが無い時は出力されない()
    {
        new ServiceLikeLoggerWrapper(null).ErrorOnPublish(new Exception(),
            MediatorListenerDescription.Create(typeof(Pang)).First());
    }

    [Fact]
    public void ErrorOnPublish_Errorがfalseの場合は出力されない()
    {
        var source = new TestServiceLikeLogger();
        source.ErrorEnabled = false;
        new ServiceLikeLoggerWrapper(source).ErrorOnPublish(new Exception(),
            MediatorListenerDescription.Create(typeof(Pang)).First());

        source.ErrorMessages.Should().BeEmpty();
    }

    [Fact]
    public void ErrorOnPublish_Errorがtrueの場合は出力される()
    {
        var source = new TestServiceLikeLogger();
        source.ErrorEnabled = true;
        new ServiceLikeLoggerWrapper(source).ErrorOnPublish(new Exception(),
            MediatorListenerDescription.Create(typeof(Pang)).First());

        source.ErrorMessages.Should().HaveCount(1);
    }

    [Fact]
    public void TraceStartPublishToListeners_元のロガーが無い時は出力されない()
    {
        new ServiceLikeLoggerWrapper(null).TraceStartPublishToListeners("test",
            MediatorListenerDescription.Create(typeof(Pang)));
    }

    [Fact]
    public void TraceStartPublishToListeners_Traceがfalseの場合は出力されない()
    {
        var source = new TestServiceLikeLogger();
        source.TraceEnabled = false;
        new ServiceLikeLoggerWrapper(source).TraceStartPublishToListeners("test",
            MediatorListenerDescription.Create(typeof(Pang)));

        source.TraceMessages.Should().BeEmpty();
    }

    [Fact]
    public void TraceStartPublishToListeners_Traceがtrueの場合は出力される()
    {
        var source = new TestServiceLikeLogger();
        source.TraceEnabled = true;
        new ServiceLikeLoggerWrapper(source).TraceStartPublishToListeners("test",
            MediatorListenerDescription.Create(typeof(Pang)));

        source.TraceMessages.Should().HaveCount(1);
    }

    [Fact]
    public void TraceFinishPublishToListeners_元のロガーが無い時は出力されない()
    {
        new ServiceLikeLoggerWrapper(null).TraceFinishPublishToListeners("test");
    }

    [Fact]
    public void TraceFinishPublishToListeners_Traceがfalseの場合は出力されない()
    {
        var source = new TestServiceLikeLogger();
        source.TraceEnabled = false;
        new ServiceLikeLoggerWrapper(source).TraceFinishPublishToListeners("test");

        source.TraceMessages.Should().BeEmpty();
    }

    [Fact]
    public void TraceFinishPublishToListeners_Traceがtrueの場合は出力される()
    {
        var source = new TestServiceLikeLogger();
        source.TraceEnabled = true;
        new ServiceLikeLoggerWrapper(source).TraceFinishPublishToListeners("test");

        source.TraceMessages.Should().HaveCount(1);
    }

    [Fact]
    public void TracePublishToListener_元のロガーが無い時は出力されない()
    {
        new ServiceLikeLoggerWrapper(null).TracePublishToListener(
            MediatorListenerDescription.Create(typeof(Pang)).First());
    }

    [Fact]
    public void TracePublishToListener_Traceがfalseの場合は出力されない()
    {
        var source = new TestServiceLikeLogger();
        source.TraceEnabled = false;
        new ServiceLikeLoggerWrapper(source).TracePublishToListener(
            MediatorListenerDescription.Create(typeof(Pang)).First());

        source.TraceMessages.Should().BeEmpty();
    }

    [Fact]
    public void TracePublishToListener_Traceがtrueの場合は出力される()
    {
        var source = new TestServiceLikeLogger();
        source.TraceEnabled = true;
        new ServiceLikeLoggerWrapper(source).TracePublishToListener(
            MediatorListenerDescription.Create(typeof(Pang)).First());

        source.TraceMessages.Should().HaveCount(1);
    }

    [Fact]
    public void ErrorOnPublishEvents_元のロガーが無い時は出力されない()
    {
        new ServiceLikeLoggerWrapper(null).ErrorOnPublishEvents(new Exception(), "test");
    }

    [Fact]
    public void ErrorOnPublishEvents_Errorがfalseの場合は出力されない()
    {
        var source = new TestServiceLikeLogger();
        source.ErrorEnabled = false;
        new ServiceLikeLoggerWrapper(source).ErrorOnPublishEvents(new Exception(), "test");

        source.ErrorMessages.Should().BeEmpty();
    }

    [Fact]
    public void ErrorOnPublishEvents_Errorがtrueの場合は出力される()
    {
        var source = new TestServiceLikeLogger();
        source.ErrorEnabled = true;
        new ServiceLikeLoggerWrapper(source).ErrorOnPublishEvents(new Exception(), "test");

        source.ErrorMessages.Should().HaveCount(1);
    }

    [Fact]
    public void HandleException_元のロガーが無い時は出力されない()
    {
        new ServiceLikeLoggerWrapper(null).HandleException(new Exception());
    }

    [Fact]
    public void HandleException_Errorがfalseの場合は出力されない()
    {
        var source = new TestServiceLikeLogger();
        source.ErrorEnabled = false;
        new ServiceLikeLoggerWrapper(source).HandleException(new Exception());

        source.ErrorMessages.Should().BeEmpty();
    }

    [Fact]
    public void HandleException_Errorがtrueの場合は出力される()
    {
        var source = new TestServiceLikeLogger();
        source.ErrorEnabled = true;
        new ServiceLikeLoggerWrapper(source).HandleException(new Exception());

        source.ErrorMessages.Should().HaveCount(1);
    }

    private class TestServiceLikeLogger : IServiceLikeLogger
    {
        public List<string> ErrorMessages = new();
        public List<string> InfoMessages = new();
        public List<string> TraceMessages = new();
        public bool ErrorEnabled = true;
        public bool InfoEnabled = true;
        public bool TraceEnabled = true;

        public void Error(Exception ex, string message, params object[] args)
            => ErrorMessages.Add(message);

        public void Info(string message, params object[] args)
        => InfoMessages.Add(message);

        public bool IsErrorEnabled() => ErrorEnabled;

        public bool IsInfoEnabled() => InfoEnabled;

        public bool IsTraceEnabled() => TraceEnabled;

        public void Trace(string message, params object[] args)
        => TraceMessages.Add(message);
    }
}
