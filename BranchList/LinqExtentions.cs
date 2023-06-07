using System;
using System.Collections.Generic;
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
    }
}
