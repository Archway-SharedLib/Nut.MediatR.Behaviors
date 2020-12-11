using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike
{
    public class DefaultMediatorClient : IMediatorClient
    {
        private readonly IMediator mediator;
        private readonly RequestRegistry registry;
        private readonly ServiceFactory factory;

        public DefaultMediatorClient(IMediator mediator, RequestRegistry registry, ServiceFactory factory)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<TResult?> SendAsync<TResult>(string path, object request) where TResult : class
        {
            var mediatorRequest = registry.GetRequest(path);
            if(mediatorRequest == null)
            {
                throw new InvalidOperationException("Mediator request was not found.");
            }
            object value = TranslateType(request, mediatorRequest.RequestType);
            var result = await mediator.Send(value).ConfigureAwait(false);

            // Unit 型や null の取り扱いをどうするか。

            return TranslateType(result, typeof(TResult)) as TResult;
        }

        private object TranslateType(object? value, Type type)
        {
            var json = JsonSerializer.Serialize(value, value.GetType(), new JsonSerializerOptions()
            {
            });
            return JsonSerializer.Deserialize(json, type);
        }
    }
}
