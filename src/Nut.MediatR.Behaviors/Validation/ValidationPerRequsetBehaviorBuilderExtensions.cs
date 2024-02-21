using System;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR;

public static class ValidationPerRequsetBehaviorBuilderExtensions
{
    public static PerRequsetBehaviorBuilder AddDataAnnotationValidation(this PerRequsetBehaviorBuilder builder)
    {
        builder.AddOpenBehavior(typeof(DataAnnotationValidationBehavior<,>));
        return builder;
    }
}
