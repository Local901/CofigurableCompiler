using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing.Simple.Stack
{
    public interface ParseStack<ITEM>
    {
        StackInterface GetRoot();
        /// <summary>
        /// Get all items between from and to excluding from, including to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>List of items stored in between them.</returns>
        IEnumerable<ITEM> GetBetween(StackInterface from, StackInterface to);

        public interface StackInterface
        {
            ITEM Item { get; }
            StackInterface Add(ITEM item);
            StackInterface AddRange(IEnumerable<ITEM> items);
            void Drop();
            StackInterface CopyInterface();
        }
    }
}
