using MediatR;

namespace Nut.MediatR.ServiceLike.DependencyInjection.Test;

[AsService("/ping")]
public class ServicePing : IRequest<Pong>
{
}

[AsService("/ping/void")]
public class VoidServicePing : IRequest { }

public class NonServicePing : IRequest<Pong>
{
}

public class VoidNonServicePing : IRequest
{
}

public class Pong
{
}
