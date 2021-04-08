using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike
{
    public interface IMediatorClient
    {
        Task<TResult?> SendAsync<TResult>(string path, object request) where TResult : class;

        Task SendAsync(string path, object request);

        Task PublishAsync(string key, object @eventData);

        Task PublishAsync(string key, object @eventData, PublishOptions options);

        Task PublishAsync(string key, object @eventData, Action<PublishOptions> options);
    }
}
