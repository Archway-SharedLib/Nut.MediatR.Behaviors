using MediatR;
using Nut.MediatR;
using System.ComponentModel.DataAnnotations;

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
