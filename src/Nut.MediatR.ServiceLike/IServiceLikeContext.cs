using System.Collections.Generic;

namespace Nut.MediatR.ServiceLike;

public interface IServiceLikeContext
{
    string Id { get; }

    string Key { get; }

    IDictionary<string, object> Header { get; }

    long Timestamp { get; }
}
