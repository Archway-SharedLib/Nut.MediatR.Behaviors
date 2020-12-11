using MediatR;
using Nut.MediatR.ServiceLike;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLikeSample.Sample.Basic
{
    [AsService("/basic")]
    public class BasicRequest: IRequest<BasicResult>
    {
        public string Id { get; set; }
    }
}
