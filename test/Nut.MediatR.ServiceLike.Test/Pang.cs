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

}
