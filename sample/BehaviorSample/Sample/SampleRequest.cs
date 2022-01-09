using System.ComponentModel.DataAnnotations;
using MediatR;
using Nut.MediatR;

namespace BehaviorSample.Sample
{
    [WithBehaviors(
        typeof(LoggingBehavior<,>),
        typeof(AuthorizationBehavior<,>),
        typeof(DataAnnotationValidationBehavior<,>),
        typeof(FluentValidationBehavior<,>))]
    public class SampleRequest : IRequest<SampleResponse>
    {
        [Required]
        public string Value { get; set; }
    }
}
