using System;
using MediatR;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

public class RequestContext
{
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
