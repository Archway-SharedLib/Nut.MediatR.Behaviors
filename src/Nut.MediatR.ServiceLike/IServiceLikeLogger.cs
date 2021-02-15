using System;

namespace Nut.MediatR.ServiceLike
{
    public interface IServiceLikeLogger
    {
        void Info(string message);

        void Error(string message, Exception ex);
    }
}