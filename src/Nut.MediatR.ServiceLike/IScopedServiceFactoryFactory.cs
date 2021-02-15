namespace Nut.MediatR.ServiceLike
{
    public interface IScopedServiceFactoryFactory
    {
        IServiceFactoryScope Create();
    }
}