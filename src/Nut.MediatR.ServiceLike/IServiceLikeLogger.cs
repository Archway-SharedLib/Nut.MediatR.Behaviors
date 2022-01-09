using System;

namespace Nut.MediatR.ServiceLike;

public interface IServiceLikeLogger
{
    void Info(string message, params object[] args);

    void Error(Exception ex, string message, params object[] args);

    void Trace(string message, params object[] args);

    bool IsTraceEnabled();

    bool IsInfoEnabled();

    bool IsErrorEnabled();
}
