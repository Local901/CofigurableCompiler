using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BranchList
{
    public class TypeBranchNode<TActual, TReturn> : BranchNode<TypeBranchNode<TActual, TReturn>> 
        where TActual : TypeBranchNode<TActual, TReturn>, TReturn
        where TReturn : class
    {
        public TReturn Parent
        {
            get => base.Parent as TReturn;
            protected set
            {
                if (value is TActual) base.Parent = (TActual)value;
                throw new Exception("Provided value for Parent was not not of correct type");
            }
        }
        public List<TReturn> Children { get => base.Children.Cast<TReturn>().ToList(); }

        public TReturn this[int index] { get => base[index] as TReturn; }

        public void Add(TReturn node)
        {
            if (node is TActual) base.Add(node as TActual);
            throw new ArgumentException($"Object to be added was not of type: {typeof(TActual).FullName}");
        }
        public void AddRange(IEnumerable<TReturn> nodes)
        {
            base.AddRange(nodes.OfType<TActual>());
        }
        public bool Contains(TReturn item)
        {
            return base.Contains((TActual)item);
        }
        public void CopyTo(TReturn[] array, int arrayIndex)
        {
            base.CopyTo((TActual[])array, arrayIndex);
        }
        public void Insert(int index, TReturn item)
        {
            base.Insert(index, (TActual)item);
        }
        public bool Remove(TReturn item)
        {
            return base.Remove((TActual)item);
        }

        public List<TReturn> Path()
        {
            return base.Path().Cast<TReturn>().ToList();
        }
        public List<TReturn> Path(Func<TReturn, bool> isBlockLike)
        {
            return base.Path((block) => isBlockLike((TActual)block)).Cast<TReturn>().ToList();
        }
        public List<TReturn> AllFirst(Func<TReturn, bool> test)
        {
            return base.AllFirst(block => test((TActual)block)).Cast<TReturn>().ToList();
        }
        public List<TReturn> Ends()
        {
            return base.Ends().Cast<TReturn>().ToList();
        }
        public List<TReturn> All()
        {
            return base.All().Cast<TReturn>().ToList();
        }
    }
}
