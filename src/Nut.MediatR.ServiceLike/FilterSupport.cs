using System;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    internal class FilterSupport
    {
        public static void ThrowIfInvalidFileterTypeAllWith(IEnumerable<Type> filterTypes)
        {
            var result = IsValidTIlterTypeAllCore(filterTypes);
            if(!result)
            {
                throw new ArgumentException("TypeはIFilterを実装しているデフォルトコンストラクタを持ったクラスでなければなりません。");
            }
        }

        public static bool IsValidTIlterTypeAllCore(IEnumerable<Type> filterTypes)
        {
            if (filterTypes is null)
            {
                throw new ArgumentNullException(nameof(filterTypes));
            }

            foreach (var type in filterTypes)
            {
                if (type is null) return false;
                if (!IsFilterType(type)) return false;
            }
            return true;
        }

        //public static IFilter Activate(Type type)
        //{

        //}

        private static bool IsFilterType(Type behaviorType)
            => behaviorType.IsImplemented(typeof(IFilter)) && behaviorType.HasDefaultConstructor();
    }
}
