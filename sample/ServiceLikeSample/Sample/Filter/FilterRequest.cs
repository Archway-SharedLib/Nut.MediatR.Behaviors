using MediatR;
using Nut.MediatR.ServiceLike;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLikeSample.Sample.Filter
{
    [AsService("/filter", typeof(ParamAndResultValueConverterFilter))]
    public class FilterRequest: IRequest<FilterResult>
    {
        public string Id { get; set; }
    }
}
