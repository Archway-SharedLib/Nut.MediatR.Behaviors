using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using Nut.MediatR.ServiceLike.Internals;
using System.Linq;

namespace Nut.MediatR.ServiceLike
{
    public class DefaultMediatorClient : IMediatorClient
    {
        private readonly IMediator mediator;
        private readonly ServiceRegistry serviceRegistry;
        private readonly ListenerRegistry listenerRegistry;
        private readonly ServiceFactory factory;
        private readonly IScopedServiceFactoryFactory scopedServiceFactoryFactory;
        private readonly ServiceLikeLoggerWrapper logger;

        [ExcludeFromCodeCoverage]
        [Obsolete("This constructor is not support the AsEvent feature. Therefore, it will be removed in v0.4.0. Please use ctor(RequestRegistry, EventRegistry, ServiceFactory, IScopedServiceFactoryFactory).")]
        public DefaultMediatorClient(IMediator mediator, ServiceRegistry registry, ServiceFactory factory)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.serviceRegistry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.listenerRegistry = new ListenerRegistry();
            this.scopedServiceFactoryFactory = new InternalScopedServiceFactoryFactory(factory);
            this.logger = new ServiceLikeLoggerWrapper(null);
        }

        [ExcludeFromCodeCoverage]
        [Obsolete("This constructor is not support the AsEvent feature. Therefore, it will be removed in v0.4.0. Please use ctor(RequestRegistry, EventRegistry, ServiceFactory, IScopedServiceFactoryFactory).")]
        public DefaultMediatorClient(ServiceRegistry serviceRegistry, ListenerRegistry eventRegistry, ServiceFactory factory)
        {
            this.serviceRegistry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
            this.listenerRegistry = eventRegistry ?? throw new ArgumentNullException(nameof(eventRegistry));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mediator = new ServiceLikeMediator(factory);
            this.scopedServiceFactoryFactory = new InternalScopedServiceFactoryFactory(factory);
            this.logger = new ServiceLikeLoggerWrapper(null);
        }
        
        public DefaultMediatorClient(ServiceRegistry serviceRegistry, ListenerRegistry eventRegistry, 
            ServiceFactory serviceFactory, IScopedServiceFactoryFactory scopedServiceFactoryFactory,
            IServiceLikeLogger logger)
        {
            factory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));;
            this.serviceRegistry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
            this.listenerRegistry = eventRegistry ?? throw new ArgumentNullException(nameof(eventRegistry));
            this.scopedServiceFactoryFactory = scopedServiceFactoryFactory ?? throw new ArgumentNullException(nameof(scopedServiceFactoryFactory));
            this.logger = new ServiceLikeLoggerWrapper(logger);
            this.mediator = new Mediator(serviceFactory);
        }

        public async Task<TResult?> SendAsync<TResult>(string path, object request) where TResult : class
        {
            var result = await SendAsyncInternal(path, request, typeof(TResult));
            return TranslateType(result, typeof(TResult)) as TResult;
        }

        public async Task SendAsync(string path, object request)
            => await SendAsyncInternal(path, request, null);

        private async Task<object?> SendAsyncInternal(string path, object request, Type? resultType)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var mediatorRequest = serviceRegistry.GetService(path);
            if(mediatorRequest is null)
            {
                throw new InvalidOperationException(SR.MediatorRequestNotFound(path));
            }
            var value = TranslateType(request, mediatorRequest.ServiceType);

            var context = new RequestContext(mediatorRequest.Path, mediatorRequest.ServiceType, factory, resultType);
            
            return await ExecuteAsync(new Queue<Type>(mediatorRequest.Filters), value, context).ConfigureAwait(false);
        }

        private object? TranslateType(object? value, Type type)
        {
            if (value is null or Unit) return null;
            var json = JsonSerializer.Serialize(value, value.GetType(), new JsonSerializerOptions());
            return JsonSerializer.Deserialize(json, type);
        }

        private async Task<object?> ExecuteAsync(Queue<Type> filterTypes, object? parameter, RequestContext context)
        {
            if (filterTypes.TryDequeue(out Type filterType))
            {
                var filter = filterType.Activate<IMediatorServiceFilter>();
                return await filter.HandleAsync(context, parameter, async (newParam) => 
                    await ExecuteAsync(filterTypes, newParam, context).ConfigureAwait(false)
                ).ConfigureAwait(false);
            }
            return await mediator.Send(parameter!).ConfigureAwait(false);
        }

        [ExcludeFromCodeCoverage]
        [Obsolete("This method will be removed in the v0.4.0. It always raises no exceptions.")]
        public Task PublishAsync(string key, object eventData, bool notifySendingError = false)
            => PublishAsync(key, eventData);
        
        public Task PublishAsync(string key, object eventData)
        {
            if (eventData is null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            var mediatorNotifications = listenerRegistry.GetListeners(key);
            PublishAndForget(mediatorNotifications, eventData, key);

            return Task.CompletedTask;
        }
        
        private void PublishAndForget(IEnumerable<MediatorListenerDescription> listeners, object notification, string key)
        {
            Task.Run(async () =>
            {
                try
                {
                    var listenersList = listeners.ToList();
                    logger.TraceStartPublishToListeners(key, listenersList);

                    using var scope = scopedServiceFactoryFactory.Create();
                    var publishTasks = new List<Task>();
                    var serviceLikeMediator = new ServiceLikeMediator(scope.Instance);

                    foreach (var listener in listenersList)
                    {
                        try
                        {
                            var value = TranslateType(notification, listener.ListenerType);
                            logger.TracePublishToListener(listener);
                            publishTasks.Add(FireEvent(listener, serviceLikeMediator, value!));
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorOnPublish(ex, listener);
                        }
                    }
                    await Task.WhenAll(publishTasks);

                    logger.TraceFinishPublishToListeners(key);
                }
                catch (Exception e)
                {
                    logger.ErrorOnPublishEvents(e, key);
                }
            });
        }

        private Task FireEvent(MediatorListenerDescription description, ServiceLikeMediator serviceLikeMediator,
            object eventData)
            => description.MediateType == MediateType.Notification
                ? serviceLikeMediator.Publish(eventData)
                : serviceLikeMediator.Send(eventData);
    }
}
