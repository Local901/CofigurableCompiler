using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Parsing.Simple3.Stack
{
    public class LinkedParseStack<ITEM> : ParseStack<ITEM>
    {
        private ParseStack<ITEM>.StackInterface Root = new Node(null);

        public IEnumerable<ITEM> GetBetween(ParseStack<ITEM>.StackInterface from, ParseStack<ITEM>.StackInterface to)
        {
            if (!(from is Node fromNode && to is Node toNode))
            {
                throw new ArgumentException("'from' or 'to' stack interface is from different parse stack.");
            }
            List<ItemNode> nodes = new List<ItemNode>();
            ItemNode? current = toNode as ItemNode;
            while (current != null && current != fromNode)
            {
                nodes.Add(current);
                current = current.Parent as ItemNode;
            }
            return nodes.Select((n) => n.Item).Reverse();
        }

        public ParseStack<ITEM>.StackInterface GetRoot()
        {
            return Root;
        }

        private class Node : ParseStack<ITEM>.StackInterface
        {
            public Node? Parent { get; }

            public virtual ITEM Item => throw new Exception("No Item in root node.");

            public Node(Node? parent)
            {
                Parent = parent;
            }

            public ParseStack<ITEM>.StackInterface Add(ITEM item)
            {
                return new ItemNode(this, item);
            }

            public ParseStack<ITEM>.StackInterface AddRange(IEnumerable<ITEM> items)
            {
                var node = this;
                foreach (var item in items)
                {
                    node = new ItemNode(node, item);
                }
                return node;
            }

            public ParseStack<ITEM>.StackInterface CopyInterface()
            {
                return this;
            }

            public void Drop()
            {
                return;
            }
        }

        private class ItemNode : Node
        {
            public override ITEM Item { get; }

            public ItemNode(Node? parent, ITEM item)
                : base(parent)
            {
                Item = item;
            }
        }
    }
}
