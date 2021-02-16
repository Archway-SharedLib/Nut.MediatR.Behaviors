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

        Task PublishAsync(string key, object notification);
        
        [Obsolete("This method will be removed in v0.4.0. It always raises no exceptions.")]
        Task PublishAsync(string key, object notification, bool notifySendingError = false);
    }
}
