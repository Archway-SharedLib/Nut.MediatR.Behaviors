using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using Nut.MediatR.ServiceLike.Internals;

namespace Nut.MediatR.ServiceLike
{
    public class DefaultMediatorClient : IMediatorClient
    {
        private readonly IMediator mediator;
        private readonly RequestRegistry requestRegistry;
        private readonly NotificationRegistry eventRegistry;
        private readonly ServiceFactory factory;
        private readonly IScopedServiceFactoryFactory scopedServiceFactoryFactory;
        private readonly IServiceLikeLogger? logger = null;

        [ExcludeFromCodeCoverage]
        [Obsolete("This constructor is not support the AsEvent feature. Therefore, it will be removed in v0.4.0. Please use ctor(RequestRegistry, EventRegistry, ServiceFactory, IScopedServiceFactoryFactory).")]
        public DefaultMediatorClient(IMediator mediator, RequestRegistry registry, ServiceFactory factory)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.requestRegistry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.eventRegistry = new NotificationRegistry();
            this.scopedServiceFactoryFactory = new InternalScopedServiceFactoryFactory(factory);
        }

        [ExcludeFromCodeCoverage]
        [Obsolete("This constructor is not support the AsEvent feature. Therefore, it will be removed in v0.4.0. Please use ctor(RequestRegistry, EventRegistry, ServiceFactory, IScopedServiceFactoryFactory).")]
        public DefaultMediatorClient(RequestRegistry requestRegistry, NotificationRegistry eventRegistry, ServiceFactory factory)
        {
            this.requestRegistry = requestRegistry ?? throw new ArgumentNullException(nameof(requestRegistry));
            this.eventRegistry = eventRegistry ?? throw new ArgumentNullException(nameof(eventRegistry));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mediator = new ServiceLikeMediator(factory);
            this.scopedServiceFactoryFactory = new InternalScopedServiceFactoryFactory(factory);
        }
        
        public DefaultMediatorClient(RequestRegistry requestRegistry, NotificationRegistry eventRegistry, 
            ServiceFactory serviceFactory, IScopedServiceFactoryFactory scopedServiceFactoryFactory,
            IServiceLikeLogger logger)
        {
            factory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));;
            this.requestRegistry = requestRegistry ?? throw new ArgumentNullException(nameof(requestRegistry));
            this.eventRegistry = eventRegistry ?? throw new ArgumentNullException(nameof(eventRegistry));
            this.scopedServiceFactoryFactory = scopedServiceFactoryFactory ?? throw new ArgumentNullException(nameof(scopedServiceFactoryFactory));
            this.logger = logger;
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

            var mediatorRequest = requestRegistry.GetRequest(path);
            if(mediatorRequest is null)
            {
                throw new InvalidOperationException(SR.MediatorRequestNotFound(path));
            }
            var value = TranslateType(request, mediatorRequest.RequestType);

            var context = new RequestContext(mediatorRequest.Path, mediatorRequest.RequestType, factory, resultType);
            
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
        public Task PublishAsync(string key, object notification, bool notifySendingError = false)
            => PublishAsync(key, notification);
        
        public Task PublishAsync(string key, object notification)
        {
            if (notification is null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var mediatorNotifications = eventRegistry.GetNotifications(key);
            PublishAndForget(mediatorNotifications, notification);

            return Task.CompletedTask;
        }
        
        private void PublishAndForget(IEnumerable<MediatorNotification> notifications, object notification)
        {
            Task.Run(async () =>
            {
                using var scope = scopedServiceFactoryFactory.Create();
                var publishTasks = new List<Task>();
                var serviceLikeMediator = new ServiceLikeMediator(scope.ServiceFactory);

                foreach (var mediatorNotification in notifications)
                {
                    try
                    {
                        var value = TranslateType(notification, mediatorNotification.NotificationType);
                        publishTasks.Add(serviceLikeMediator.Publish(value!));
                    }
                    catch (Exception ex)
                    {
                        logger?.Error(SR.Client_RaiseExWhenPublish(mediatorNotification.NotificationType.Name), ex);
                    }
                }
                await Task.WhenAll(publishTasks);
            });
        }
    }
}
