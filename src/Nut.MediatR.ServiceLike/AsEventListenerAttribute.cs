using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    [ExcludeFromCodeCoverage]
    [Obsolete("This class will be removed in the v0.4.0. Please use AsEventListenerAttribute.")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AsEventAttribute: Attribute
    {
        public AsEventAttribute(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(key)));
            }

            this.Path = key;     
        }
       
        public string Path { get; }
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AsEventListenerAttribute: Attribute
    {
        public AsEventListenerAttribute(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(key)));
            }

            this.Path = key;     
        }
       
        public string Path { get; }
    }
}
