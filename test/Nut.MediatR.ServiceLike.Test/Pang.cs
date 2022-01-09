using MediatR;

namespace Nut.MediatR.ServiceLike.Test;

[AsEventListener("pang")]
public record Pang : INotification
{
}

#pragma warning disable 618
[AsEventListener("pang")]
public record Pang2 : INotification
{
}

[AsEventListener("pang.1")]
[AsEventListener("pang.2")]
public record MultiPang : INotification
{
}

[AsEventListener("pang.open.generic")]
public class OpenGenericPang<T> : INotification { }

[AsEventListener("pang.abstract")]
public abstract class AbstractPang : INotification { }

[AsEventListener("pang.plain")]
public class PlainPang { }

public class OnlyPang { }

[AsEventListener("pang.request")]
public record RequestPang : IRequest;

[AsEventListener("pang.requestT")]
public record RequestTPang : IRequest<Pong>;
