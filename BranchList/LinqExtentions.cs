using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BranchList
{
    public static class LinqExtentions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            var enumerotor = list.GetEnumerator();
            while (enumerotor.MoveNext())
            {
                action(enumerotor.Current);
            }
            return list;
        }

        public static IEnumerable<R> FilterCast<T,R>(this IEnumerable<T> list)
        {
            return list.Where(item => item is R)
                .Cast<R>();
        }
    }
}
