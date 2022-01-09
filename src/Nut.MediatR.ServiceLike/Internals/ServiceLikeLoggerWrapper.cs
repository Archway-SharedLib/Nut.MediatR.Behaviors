using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike.Internals;

internal class ServiceLikeLoggerWrapper
{
    private readonly IServiceLikeLogger? sourceLogger;

    public ServiceLikeLoggerWrapper(IServiceLikeLogger? sourceLogger)
    {
        this.sourceLogger = sourceLogger;
    }

    public void ErrorOnPublish(Exception ex, MediatorListenerDescription listener)
    {
        if (sourceLogger?.IsErrorEnabled() == true)
        {
            sourceLogger.Error(ex, SR.Client_RaiseExWhenEachListener(listener.Key, listener.ListenerType.Name, listener.MediateType));
        }
    }

    internal void TraceStartPublishToListeners(string key, IEnumerable<MediatorListenerDescription> listeners)
    {
        if (sourceLogger?.IsTraceEnabled() == true)
        {
            sourceLogger.Trace(SR.Client_BeforePublishToListeners(key, listeners.Count()));
        }
    }

    internal void TraceFinishPublishToListeners(string key)
    {
        if (sourceLogger?.IsTraceEnabled() == true)
        {
            sourceLogger.Trace(SR.Client_CompletePublishToListeners(key));
        }
    }

    internal void TracePublishToListener(MediatorListenerDescription listener)
    {
        if (sourceLogger?.IsTraceEnabled() == true)
        {
            sourceLogger.Trace(SR.Client_PublishToEachListeners(listener.Key, listener.ListenerType.Name, listener.MediateType));
        }
    }

    internal void ErrorOnPublishEvents(Exception ex, string key)
    {
        if (sourceLogger?.IsErrorEnabled() == true)
        {
            sourceLogger.Error(ex, SR.Client_RaizeExWhenPublish(key));
        }
    }
}
