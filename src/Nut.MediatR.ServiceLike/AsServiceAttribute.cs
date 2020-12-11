using System;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AsServiceAttribute: Attribute
    {
        public AsServiceAttribute(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' を null または空白にすることはできません", nameof(path));
            }
            this.Path = path;
        }

        public string Path { get; }
    }
}
