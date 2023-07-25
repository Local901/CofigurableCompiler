using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BranchList
{
    public interface IBranchNode<T> where T : IBranchNode<T>
    {
        public T Parent { get; }
        public List<T> Children { get; }

        public T this[int index] { get; }

        public void Add(T node);
        public void AddRange(IEnumerable<T> nodes);
        public void Clear();
        public bool Contains(T item);
        public void CopyTo(T[] array, int arrayIndex);
        public IEnumerator GetEnumerator();
        public void Insert(int index, T item);
        public bool Remove(T item);
        public void RemoveAt(int index);

        /// <summary>
        /// Get the path from the root of the tree to this node.
        /// </summary>
        /// <returns>A list starting with the root and ending with the node that this function was called on.</returns>
        public List<T> Path();
        /// <summary>
        /// Get the path from the first parent that return true on the test to this node.
        /// </summary>
        /// <returns>A list starting with the root and ending with the node that this function was called on.</returns>
        public List<T> Path(Func<T, bool> isBlockLike);
        /// <summary>
        /// Get a list of all first incounters in a branch that return true to the test function.
        /// </summary>
        /// <param name="test">A function to check the nodes.</param>
        /// <returns></returns>
        public List<T> AllFirst(Func<T, bool> test);
        /// <summary>
        /// Get a list of all the nodes where the bances of the tree ends.
        /// </summary>
        /// <returns>A list of al the ending nodes.</returns>
        public List<T> Ends();
        /// <summary>
        /// Get a list of all the nodes in the tree.
        /// </summary>
        /// <returns></returns>
        public List<T> All();
    }
}
