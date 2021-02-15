using System;
using MediatR;

namespace Nut.MediatR.ServiceLike
{
    public interface IServiceFactoryScope: IDisposable
    {
        ServiceFactory ServiceFactory { get; }
    }
}