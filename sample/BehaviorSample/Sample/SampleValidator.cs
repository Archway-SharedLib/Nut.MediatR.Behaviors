using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BehaviorSample.Sample
{
    public class SampleValidator : AbstractValidator<SampleRequest>
    {
        public SampleValidator()
        {
            RuleFor(v => v.Value).MaximumLength(20);
        }
    }
}
