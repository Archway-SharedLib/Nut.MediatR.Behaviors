using System;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike
{   
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
