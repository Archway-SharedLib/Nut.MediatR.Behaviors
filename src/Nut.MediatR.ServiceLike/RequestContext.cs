using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    public class RequestContext
    {
        [ExcludeFromCodeCoverage]
        [Obsolete("This method will be removed in the v0.4.0. It always raises no exceptions.Please use ctor(string, Type, ServiceFactory, Type?).")]
        public RequestContext(string path, Type mediatorParameterType, Type clientResultType , ServiceFactory serviceFactory )
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(path)));
            }
            Path = path;
            MediatorParameterType = mediatorParameterType ?? throw new ArgumentNullException(nameof(mediatorParameterType));
            ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            ClientResultType = clientResultType;
        }
        
        public RequestContext(string path, Type mediatorParameterType, ServiceFactory serviceFactory, Type? clientResultType = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(path)));
            }
            Path = path;
            MediatorParameterType = mediatorParameterType ?? throw new ArgumentNullException(nameof(mediatorParameterType));
            ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            ClientResultType = clientResultType;
        }

        public string Path { get; }
        
        public Type MediatorParameterType { get; }
        
        public Type? ClientResultType { get; }

        public bool NeedClientResult => ClientResultType is not null;

        public ServiceFactory ServiceFactory { get; }
    }
}
