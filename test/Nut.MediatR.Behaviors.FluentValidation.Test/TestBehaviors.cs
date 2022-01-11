using MediatR;

namespace Nut.MediatR.Behaviors.FluentValidation.Test;

public class TestBehaviorRequest : IRequest<TestBehaviorResponse>
{
    public string Name { get; set; }

    public int Age { get; set; }
}

public class TestBehaviorResponse
{
    public string Value { get; set; }
}
