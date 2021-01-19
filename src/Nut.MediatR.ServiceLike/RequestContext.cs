using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    public class RequestContext
    {
        public RequestContext(string path, Type mediatorParameterType, Type clientResultType, ServiceFactory serviceFactory)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(path)));
            }
            Path = path;
            MediatorParameterType = mediatorParameterType ?? throw new ArgumentNullException(nameof(mediatorParameterType));
            ClientResultType = clientResultType ?? throw new ArgumentNullException(nameof(clientResultType));
            ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public string Path { get; }
        
        public Type MediatorParameterType { get; }
        
        public Type ClientResultType { get; }
        
        public ServiceFactory ServiceFactory { get; }
    }
}
