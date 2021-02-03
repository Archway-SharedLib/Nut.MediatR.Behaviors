using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike.DependencyInjection.Test
{
    [AsEvent("pang")]
    public class Pang: INotification
    {
    }

    [AsEvent("pang2")]
    public class Pang2 : INotification
    {
    }
}
