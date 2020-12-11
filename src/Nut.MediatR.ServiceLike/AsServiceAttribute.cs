using System;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AsServiceAttribute: Attribute
    {
        public AsServiceAttribute(string path)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string Path { get; }
    }
}
