using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BranchList
{
    public class BranchNode<T> : IBranchNode<T> where T : BranchNode<T>
    {
        public T Parent { get; protected set; }
        public List<T> Children { get; }

        public BranchNode()
        {
            Children = new List<T>();
        }
        public BranchNode(List<T> children)
        {
            Children = children;
        }

        public T this[int index] { get => Children[index]; protected set => Children[index] = value; }

        public bool IsFixedSize => ((IList)Children).IsFixedSize;

        public bool IsReadOnly => ((IList)Children).IsReadOnly;

        public int Count => ((ICollection)Children).Count;

        public bool IsSynchronized => ((ICollection)Children).IsSynchronized;

        public object SyncRoot => ((ICollection)Children).SyncRoot;

        public void Add(T node)
        {
            if (node.Parent != null)
            {
                throw new ArgumentException("Parent of node should be null when added.");
            }
            Children.Add(node);
            node.Parent = (T)this;
        }

        public void AddRange(IEnumerable<T> nodes)
        {
            nodes.ForEach(node => Add(node));
        }

        public void Clear()
        {
            Children.Clear();
        }

        public bool Contains(T item)
        {
            if (Children.Contains(item))
                return true;
            return Children.Any(b => b.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public IEnumerator GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public void Insert(int index, T item)
        {
            Children.Insert(index, item);
            item.Parent = (T)this;
        }

        public bool Remove(T item)
        {
            item.Parent = null;
            return Children.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Children[index].Parent = null;
            Children.RemoveAt(index);
        }

        public List<T> Path()
        {
            List<T> path = Parent == null
                ? new List<T>()
                : Parent.Path();
            path.Add((T)this);
            return path;
        }
        public List<T> AllFirst(Func<T, bool> test)
        {
            if (test((T)this))
            {
                return new List<T>{ (T)this };
            }

            return Children.SelectMany((node) => node.AllFirst(test)).ToList();
        }
        public List<T> Ends()
        {
            List<T> result = new List<T>();

            if (Count != 0)
            {
                ForEach(b => result.AddRange(b.Ends()));
            }
            else
            {
                result.Add((T)this);
            }

            return result;
        }
        public List<T> All()
        {
            List<T> result = new List<T>();
            if (this is T)
            {
                result.Add((T)this);
            }
            result.AddRange(Children.SelectMany(c => c.All()));
            return result;
        }

        public IEnumerable<T> ForEach(Action<T> action)
        {
            Children.ForEach(action);
            return Children;
        }
    }
}
