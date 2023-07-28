using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BranchList
{
    public static class LinqExtentions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (T item in list)
            {
                action(item);
            }
        }
    }
}
