using System;
using System.Runtime.Serialization;

namespace Nut.MediatR
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}