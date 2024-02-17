using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR.Test;

public static class TestBehaviorMessages
{
    public const string StartMessage1 = "TestBehavior1 Start";
    public const string EndMessage1 = "TestBehavior1 End";
    public const string StartMessage2 = "TestBehavior2 Start";
    public const string EndMessage2 = "TestBehavior2 End";
    public const string StartMessage3 = "TestBehavior3 Start";
    public const string EndMessage3 = "TestBehavior3 End";
}

public class ExecHistory
{
    public List<string> List { get; } = new();
}

public class TestBehavior1<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ExecHistory _execHistory;


    public TestBehavior1(ExecHistory execHistory)
    {
        _execHistory = execHistory;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _execHistory.List.Add(TestBehaviorMessages.StartMessage1);
        var result = next();
        _execHistory.List.Add(TestBehaviorMessages.EndMessage1);
        return result;
    }
}

public class TestBehavior2<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ExecHistory _execHistory;

    public TestBehavior2(ExecHistory execHistory)
    {
        _execHistory = execHistory;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _execHistory.List.Add(TestBehaviorMessages.StartMessage2);
        var result = next();
        _execHistory.List.Add(TestBehaviorMessages.EndMessage2);
        return result;
    }
}

public class TestBehavior3<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ExecHistory _execHistory;

    public TestBehavior3(ExecHistory execHistory)
    {
        _execHistory = execHistory;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _execHistory.List.Add(TestBehaviorMessages.StartMessage3);
        var result = next();
        _execHistory.List.Add(TestBehaviorMessages.EndMessage3);
        return result;
    }
}

public class TestBehaviorRequest : IRequest<TestBehaviorResponse>
{
    [Required]
    [MaxLength(20)]
    public string Value { get; set; }
}

public class TestBehaviorResponse
{
    public string Value { get; set; }
}
