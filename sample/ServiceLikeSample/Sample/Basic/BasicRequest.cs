using MediatR;
using Nut.MediatR.ServiceLike;

namespace ServiceLikeSample.Sample.Basic;

[AsService("/basic")]
public class BasicRequest : IRequest<BasicResult>
{
    public string Id { get; set; }
}
