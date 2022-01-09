using MediatR;

namespace Nut.MediatR.ServiceLike.DependencyInjection.Test;

[AsEventListener("pang")]
public class Pang : INotification
{
}

[AsEventListener("pang2")]
public class Pang2 : INotification
{
}
