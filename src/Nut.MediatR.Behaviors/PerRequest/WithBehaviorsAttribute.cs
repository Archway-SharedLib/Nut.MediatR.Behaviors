using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Nut.MediatR
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public class WithBehaviorsAttribute : Attribute
    {
        public IList<Type> BehaviorTypes { get; }

        public WithBehaviorsAttribute(params Type[] behaviorTypes)
        {
            if (behaviorTypes is null) throw new ArgumentNullException(nameof(behaviorTypes));
            foreach (var type in behaviorTypes)
            {
                if (type is null) throw new ArgumentException(SR.PerRequest_ContainsNullInTypes);
                if (!IsPipelineBehaviorType(type)) throw new ArgumentException(SR.PerRequest_TypeIsNotBehavior(type.FullName, typeof(IPipelineBehavior<,>).Name));
            }
            BehaviorTypes = new ReadOnlyCollection<Type>(behaviorTypes);
        }

        private bool IsPipelineBehaviorType(Type behaviorType)
            => behaviorType
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
    }
}
