using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BranchList
{
    public class ValueBranchNode<T> : BranchNode<ValueBranchNode<T>>
    {
        public T Value;

        public ValueBranchNode(T value)
            : base()
        {
            Value = value;
        }

        public void Add(T item)
        {
            Add(new ValueBranchNode<T>(item));
        }

        public void Add(IList<T> items)
        {
            items.ForEach(i => Add(i));
        }

        public bool Contains(T item)
        {
            return Children.Any(b => b.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Children.Select(b => b.Value)
                .ToArray()
                .CopyTo(array, arrayIndex);
        }

        public void Insert(int index, T item)
        {
            Children.Insert(index, new ValueBranchNode<T>(item));
        }

        private int IndexOf(T item)
        {
            return Children.Select(b => b.Value)
                .ToList()
                .IndexOf(item);
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index > -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}
