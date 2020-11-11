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
        private readonly ServiceFactory serviceFactory;
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;
        
        public LoggingBehavior(ILoggerFactory loggerFactory, ServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            this.logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger<LoggingBehavior<TRequest, TResponse>>();
        }
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var collector = serviceFactory.GetInstance<ILoggingInOutValueCollector<TRequest, TResponse>>();
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