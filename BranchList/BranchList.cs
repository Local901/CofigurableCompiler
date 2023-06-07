using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BranchList
{
    public class BranchList<T>
    {
        public List<BranchNode<T>> Children = new List<BranchNode<T>>();

        public BranchNode<T> this[int index] { get => Children[index]; set => Children[index] = value; }

        public bool IsFixedSize => ((IList)Children).IsFixedSize;

        public bool IsReadOnly => ((IList)Children).IsReadOnly;

        public int Count => ((ICollection)Children).Count;

        public bool IsSynchronized => ((ICollection)Children).IsSynchronized;

        public object SyncRoot => ((ICollection)Children).SyncRoot;


        public void Add(T item)
        {
            Children.Add(new BranchNode<T>(item));
        }

        public void Add(BranchNode<T> item)
        {
            Children.Add(item);
        }

        public void TryAdd(IList<T> items)
        {
            if (items.Count == 0)
                return;
            if (!Contains(items[0]))
            {
                Add(items[0]);
            }
            var b = this[IndexOf(items[0])];
            b.TryAdd(items.Skip(1).ToList());
        }

        public void Clear()
        {
            Children.Clear();
        }

        public bool Contains(T item)
        {
            return Children.Any(b => b.Contains(item));
        }

        public bool Contains(BranchNode<T> item)
        {
            if (Children.Contains(item))
                return true;
            return Children.Any(b => b.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Children.Select(b => b.Value).ToArray().CopyTo(array, arrayIndex);
        }

        public void CopyTo(BranchNode<T>[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public IEnumerator GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public void Insert(int index, T item)
        {
            Children.Insert(index, new BranchNode<T>(item));
        }

        public void Insert(int index, BranchNode<T> item)
        {
            Children.Insert(index, item);
        }

        private int IndexOf(T item)
        {
            return Children.Select(b => b.Value).ToList().IndexOf(item);
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

        public bool Remove(BranchNode<T> item)
        {
            return Children.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Children.RemoveAt(index);
        }

        public List<BranchNode<T>> Ends()
        {
            List<BranchNode<T>> result = new List<BranchNode<T>>();

            if (Count != 0)
            {
                ForEach(b => result.AddRange(b.Ends()));
            }
            else if (this is BranchNode<T>)
            {
                result.Add((BranchNode<T>)this);
            }
            
            return result;
        }

        public List<BranchNode<T>> All()
        {
            List<BranchNode<T>> result = new List<BranchNode<T>>();
            if (this is BranchNode<T>)
            {
                result.Add(((BranchNode<T>)this));
            }
            result.AddRange(Children.SelectMany(c => c.All()));
            return result;
        }

        public IEnumerable<BranchNode<T>> ForEach(Action<BranchNode<T>> action)
        {
            Children.ForEach(action);
            return Children;
        }
    }
}
