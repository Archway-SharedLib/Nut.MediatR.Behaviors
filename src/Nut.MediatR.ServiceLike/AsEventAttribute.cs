using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
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
}
