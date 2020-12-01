using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Nut.MediatR
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
    {
        protected ServiceFactory ServiceFactory { get; }
        
        public LoggingBehavior(ServiceFactory serviceFactory)
        {
            this.ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        protected virtual ILoggingInOutValueCollector<TRequest, TResponse>? GetDefaultCollector() => null;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var logger = ServiceFactory.GetInstance<ILogger<LoggingBehavior<TRequest, TResponse>>>();
            if(logger == null)
            {
                return await next().ConfigureAwait(false);
            }

            var collector = ServiceFactory.GetInstance<ILoggingInOutValueCollector<TRequest, TResponse>>() 
                ?? this.GetDefaultCollector();
            
            var inValue = collector != null ? 
                await collector.CollectInValueAsync(request, cancellationToken).ConfigureAwait(false) :
                null;
            
            if (inValue?.HasValue == true)
            {
                logger.Log(LogLevel.Information, "Start {Request}. {Input}", typeof(TRequest).Name, inValue.Get());
            }
            else
            {
                logger.Log(LogLevel.Information, "Start {Request}.", typeof(TRequest).Name);
            }
            
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                var result = await next().ConfigureAwait(false);
                watch.Stop();

                var outValue = collector != null ?
                    await collector.CollectOutValueAsync(result, cancellationToken).ConfigureAwait(false) :
                    null;

                if (outValue?.HasValue == true)
                {
                    logger.Log(LogLevel.Information, "Complete {Request} in {Elapsed}ms. {Output}", 
                        typeof(TRequest).Name, 
                        watch.ElapsedMilliseconds, 
                        outValue.Get());
                }
                else
                {
                    logger.Log(LogLevel.Information, "Complete {Request} in {Elapsed}ms.", 
                        typeof(TRequest).Name, 
                        watch.ElapsedMilliseconds);
                }
                
                return result;
            }
            catch(Exception e)
            {
                watch.Stop();
                logger.Log(LogLevel.Error, e, "Exception {Request} in {Elapsed}ms.", typeof(TRequest).Name, watch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }
    }
}