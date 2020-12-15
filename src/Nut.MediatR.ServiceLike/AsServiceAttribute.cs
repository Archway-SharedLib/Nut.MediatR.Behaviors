using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AsServiceAttribute: Attribute
    {
        public AsServiceAttribute(string path, params Type[] filterTypes)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' を null または空白にすることはできません", nameof(path));
            }
            this.Path = path;

            if (filterTypes is null) throw new ArgumentNullException(nameof(filterTypes));
            FilterSupport.ThrowIfInvalidFileterTypeAllWith(filterTypes);
            
            FilterTypes = new ReadOnlyCollection<Type>(filterTypes);
        }

        

        public string Path { get; }

        public IList<Type> FilterTypes { get; }
    }
}
