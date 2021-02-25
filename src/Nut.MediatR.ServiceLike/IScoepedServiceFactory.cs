using System;
using MediatR;

namespace Nut.MediatR.ServiceLike
{
    public interface IScoepedServiceFactory: IDisposable
    {
        ServiceFactory Instance { get; }
    }
}