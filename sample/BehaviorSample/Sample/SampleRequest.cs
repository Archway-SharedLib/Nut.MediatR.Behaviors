using MediatR;
using Nut.MediatR;
using Nut.MediatR.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BehaviorSample.Sample
{
    [WithBehaviors(
        typeof(LoggingBehavior<,>),
        typeof(AuthorizationBehavior<,>),
        typeof(DataAnnotationValidationBehavior<,>),
        typeof(FluentValidationBehavior<,>))]
    public class SampleRequest: IRequest<SampleResponse>
    {
        [Required]
        public string Value { get; set; }
    }
}
