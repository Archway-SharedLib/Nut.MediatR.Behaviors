using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike
{
    public interface IMediatorClient
    {
        Task<TResult?> SendAsync<TResult>(string path, object request) where TResult : class;
    }
}
