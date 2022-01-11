using System;
using System.Collections.Generic;
using System.Linq;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike.Internals;

internal class ServiceLikeLoggerWrapper
{
    private readonly IServiceLikeLogger? _sourceLogger;

    public ServiceLikeLoggerWrapper(IServiceLikeLogger? sourceLogger)
    {
        _sourceLogger = sourceLogger;
    }

    public void ErrorOnPublish(Exception ex, MediatorListenerDescription listener)
    {
        if (_sourceLogger?.IsErrorEnabled() == true)
        {
            _sourceLogger.Error(ex, SR.Client_RaiseExWhenEachListener(listener.Key, listener.ListenerType.Name, listener.MediateType));
        }
    }

    internal void TraceStartPublishToListeners(string key, IEnumerable<MediatorListenerDescription> listeners)
    {
        if (_sourceLogger?.IsTraceEnabled() == true)
        {
            _sourceLogger.Trace(SR.Client_BeforePublishToListeners(key, listeners.Count()));
        }
    }

    internal void TraceFinishPublishToListeners(string key)
    {
        if (_sourceLogger?.IsTraceEnabled() == true)
        {
            _sourceLogger.Trace(SR.Client_CompletePublishToListeners(key));
        }
    }

    internal void TracePublishToListener(MediatorListenerDescription listener)
    {
        if (_sourceLogger?.IsTraceEnabled() == true)
        {
            _sourceLogger.Trace(SR.Client_PublishToEachListeners(listener.Key, listener.ListenerType.Name, listener.MediateType));
        }
    }

    internal void ErrorOnPublishEvents(Exception ex, string key)
    {
        if (_sourceLogger?.IsErrorEnabled() == true)
        {
            _sourceLogger.Error(ex, SR.Client_RaizeExWhenPublish(key));
        }
    }
}
