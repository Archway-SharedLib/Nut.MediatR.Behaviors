namespace Nut.MediatR.ServiceLike;

public interface IServiceLikeContextAccessor
{
    IServiceLikeContext? Context { get; set; }
}
