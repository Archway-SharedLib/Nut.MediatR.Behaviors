using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike.Test
{
    [AsEvent("pang")]
    public record Pang: INotification
    {
    }

    [AsEvent("pang")]
    public record Pang2 : INotification
    {
    }

    [AsEvent("pang.1")]
    [AsEvent("pang.2")]
    public record MultiPang : INotification
    {
    }

    [AsEvent("pang.open.generic")]
    public class OpenGenericPang<T> : INotification { }

    [AsEvent("pang.abstract")]
    public abstract class AbstractPang : INotification { }

    [AsEvent("pang.plain")]
    public class PlainPang { }

    public class OnlyPang { }

}
