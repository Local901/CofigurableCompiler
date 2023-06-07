using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BranchList
{
    public class BranchNode<T> : BranchList<T>
    {
        public T Value;
        public BranchNode<T> Parent;

        public BranchNode()
        {
            Children = new List<BranchNode<T>>();
        }
        public BranchNode(T value)
            : this()
        {
            Value = value;
        }
        public BranchNode(T value, BranchNode<T> parent)
            : this(value)
        {
            Parent = parent;
        }
    }
}
