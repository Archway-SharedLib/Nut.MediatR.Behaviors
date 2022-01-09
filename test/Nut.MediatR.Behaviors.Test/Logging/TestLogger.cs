using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Nut.MediatR.Test.Logging;

public class TestLogger<TName> : ILogger<TName>
{

    public IList<string> Scopes { get; } = new List<string>();
    public IList<TestLogInfo> Logs { get; } = new List<TestLogInfo>();

    public IDisposable BeginScope<TState>(TState state)
    {
        Scopes.Add(state?.ToString());
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter == null)
        {
            Logs.Add(new TestLogInfo(logLevel, eventId, state.ToString(), exception, state as IReadOnlyList<KeyValuePair<string, object>>));
        }
        else
        {
            Logs.Add(new TestLogInfo(logLevel, eventId, formatter(state, exception), exception, state as IReadOnlyList<KeyValuePair<string, object>>));
        }
    }
}

public class TestLogInfo
{
    public TestLogInfo(LogLevel logLevel, EventId eventId, string message, Exception exception, IReadOnlyList<KeyValuePair<string, object>> state)
    {
        LogLevel = logLevel;
        EventId = eventId;
        Message = message;
        Exception = exception;
        State = state;
    }

    public LogLevel LogLevel { get; }
    public EventId EventId { get; }
    public string Message { get; }
    public Exception Exception { get; }
    public IReadOnlyList<KeyValuePair<string, object>> State { get; }
}
