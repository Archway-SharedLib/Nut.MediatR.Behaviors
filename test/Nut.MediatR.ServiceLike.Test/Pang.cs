using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike.Test
{
    [AsEventListener("pang")]
    public record Pang: INotification
    {
    }
    
#pragma warning disable 618
    [AsEvent("pang")]
#pragma warning restore 618
    public record ObsoletePang: INotification
    {
    }


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

}
