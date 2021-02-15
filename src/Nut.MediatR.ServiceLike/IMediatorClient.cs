using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike
{
    public interface IMediatorClient
    {
        Task<TResult?> SendAsync<TResult>(string path, object request) where TResult : class;

        Task PublishAsync(string key, object notification);
        
        [Obsolete("This method will be removed in the near future. It always raises no exceptions.")]
        Task PublishAsync(string key, object notification, bool notifySendingError = false);
    }
}
