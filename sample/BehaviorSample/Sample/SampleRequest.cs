using MediatR;
using Nut.MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BehaviorSample.Sample
{
    [WithBehaviors(
        typeof(LoggingBehavior<,>),
        typeof(AuthorizationBehavior<,>),
        typeof(DataAnnotationValidationBehavior<,>))]
    public class SampleRequest: IRequest<SampleResponse>
    {
        [Required]
        public string Value { get; set; }
    }
}
