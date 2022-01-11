using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR.ServiceLike.Test;

[AsService("/ping")]
public class ServicePing : IRequest<Pong>
{
    public string Value { get; set; }
}

[AsService("/ping")]
public class ServicePing2 : IRequest<Pong>
{
    public string Value { get; set; }
}

[AsService("/ping", typeof(Filter1), typeof(Filter4))]
public class ServiceWithFilterPing : IRequest<Pong>
{
    public string Value { get; set; }
}

[AsService("/ping/1")]
[AsService("/ping/2")]
public class MultiServicePing : IRequest<Pong>
{
    public string Value { get; set; }
}

[AsService("/ping/null")]
public class ServiceNullPing : IRequest<Pong>
{
    public string Value { get; set; }
}

[AsService("/ping/void")]
public class VoidServicePing : IRequest { }

public class NonServicePing : IRequest<Pong>
{
}

public class VoidNonServicePing : IRequest
{
}

public class Pong
{
    public string Value { get; set; }
}

public class ExecuteCheck
{
    public bool Executed { get; set; } = false;
}


public class ServicePingHandler : IRequestHandler<ServicePing, Pong>
{
    private readonly ExecuteCheck check;

    public ServicePingHandler(ExecuteCheck check)
    {
        this.check = check;
    }

    public Task<Pong> Handle(ServicePing request, CancellationToken cancellationToken)
    {
        check.Executed = true;
        return Task.FromResult(new Pong() { Value = request.Value + " Pong" });
    }
}

public class ServiceNullPingHandler : IRequestHandler<ServiceNullPing, Pong>
{
    private readonly ExecuteCheck check;

    public ServiceNullPingHandler(ExecuteCheck check)
    {
        this.check = check;
    }
    public Task<Pong> Handle(ServiceNullPing request, CancellationToken cancellationToken)
    {
        check.Executed = true;
        return Task.FromResult(null as Pong);
    }
}

public class VoidServicePingHandler : IRequestHandler<VoidServicePing>
{
    private readonly ExecuteCheck check;

    public VoidServicePingHandler(ExecuteCheck check)
    {
        this.check = check;
    }
    public Task<Unit> Handle(VoidServicePing request, CancellationToken cancellationToken)
    {
        check.Executed = true;
        return Unit.Task;
    }
}

public class Filter1 : IMediatorServiceFilter
{
    public async Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
    {
        var check = context.ServiceFactory.GetInstance<FilterExecutionCheck>();
        check.Checks.Add("1");
        return await next(parameter);
    }
}

public class Filter2 : IMediatorServiceFilter
{
    public async Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
    {
        var check = context.ServiceFactory.GetInstance<FilterExecutionCheck>();
        check.Checks.Add("2");
        return await next(parameter);
    }
}

public class Filter3 : IMediatorServiceFilter
{
    public async Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
    {
        var check = context.ServiceFactory.GetInstance<FilterExecutionCheck>();
        check.Checks.Add("3");
        return await next(parameter);
    }
}

public class Filter4 : IMediatorServiceFilter
{
    public async Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
    {
        var check = context.ServiceFactory.GetInstance<FilterExecutionCheck>();
        check.Checks.Add("4");
        return await next(parameter);
    }
}

public class FilterExecutionCheck
{

    public List<string> Checks { get; set; } = new List<string>();

}
