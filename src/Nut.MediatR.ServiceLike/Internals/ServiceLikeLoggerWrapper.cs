using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nut.MediatR.ServiceLike.Internals
{
    internal class ServiceLikeLoggerWrapper
    {
        private readonly IServiceLikeLogger? sourceLogger;

        public ServiceLikeLoggerWrapper(IServiceLikeLogger? sourceLogger)
        {
            this.sourceLogger = sourceLogger;
        }

        public void ErrorOnPublish(Exception ex, MediatorListenerDescription listener)
        {
            if(this.sourceLogger?.IsErrorEnabled() == true)
            {
                this.sourceLogger?.Error(ex, SR.Client_RaiseExWhenPublish(listener.ListenerType.Name) + $"(key: {listener.Key}, type: {listener.MediateType}).");
            }
        }

        internal void TraceStartPublishToListeners(IEnumerable<MediatorListenerDescription> listeners)
        {
            if (this.sourceLogger?.IsTraceEnabled() == true)
            {
                this.sourceLogger?.Trace($"Publish mediator events to {listeners.Count()} listener(s).");
            }
        }

        internal void TraceFinishPublishToListeners()
        {
            if (this.sourceLogger?.IsTraceEnabled() == true)
            {
                this.sourceLogger?.Trace($"Published mediator events.");
            }
        }

        internal void TracePublishToListener(MediatorListenerDescription listener)
        {
            if (this.sourceLogger?.IsTraceEnabled() == true)
            {
                this.sourceLogger?.Trace($"Publishe mediator event to {listener.ListenerType.Name} (key: {listener.Key}, type: {listener.MediateType}).");
            }
        }
    }
}
