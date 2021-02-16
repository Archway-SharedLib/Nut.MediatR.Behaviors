using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike.DependencyInjection.Test
{
    [AsEventListener("pang")]
    public class Pang: INotification
    {
    }

    [AsEventListener("pang2")]
    public class Pang2 : INotification
    {
    }
}
