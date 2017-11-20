using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRefactoring2.Vsix
{
    public static class LinqHelpers
    {
        static int IntersectionCount<T>(this IEnumerable<T> __this, IEnumerable<T> that)
        {
            if (__this == null) throw new ArgumentNullException(nameof(__this));
            if (that == null) throw new ArgumentNullException(nameof(that));

            var set = new HashSet<T>(__this);
            set.IntersectWith(that);
            return set.Count;
        }
    }
}
