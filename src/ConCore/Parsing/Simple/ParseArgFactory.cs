using BranchList;
using ConCore.Blocks;
using ConCore.CustomRegex.Info;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Parsing.Simple.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Parsing.Simple
{
    public class ParseArgFactory : IParseArgFactory
    {
        private KeyCollection Collection;

        public ParseArgFactory(KeyCollection collection)
        {
            Collection = collection;
        }

        public IParseArgs CreateNextArgs(IReadOnlyList<IValueInfo<bool, Component>> componentPath, IParseArgs parent, IBlock block)
        {
            if (componentPath == null || componentPath.Count == 0) throw new ArgumentException("ComponentPath should contain at leats one element.");
            if (parent == null) throw new ArgumentException("The parent arg can't be null.");

            var currentParent = parent;
            for (int i = 0; i < componentPath.Count; i++)
            {
                var step = componentPath[i];

                // Set local root to the parent if it is a localroot and hasn't been completed yet.
                // Completion of a localroot indicates that it no longer is building itself and has become a part of it's localroot.
                var localRoot = currentParent is ILocalRoot && currentParent.Block == null
                    ? (ILocalRoot)currentParent
                    : currentParent.LocalRoot;
                IKey key = Collection.GetKey(step.Value.Reference);
                IParseArgs newParent;

                if (key is KeyGroup)
                {
                    // Don't represent a group in the tree.
                    continue;
                }
                // Add a ParseArg if it is a token (Always last).
                else if (key is Token)
                {
                    newParent = new ParseArgs(step, localRoot, block);
                }
                // Add a ConstructArg if it is a Construct (Always in between)
                else if (key is Construct)
                {
                    newParent = i == componentPath.Count - 1
                        ? new ConstructArgs((Construct)key, step, localRoot, block)
                        : new ConstructArgs((Construct)key, step, localRoot);
                }
                else
                {
                    throw new Exception("Unknown type in component path.");
                }

                currentParent.Add(newParent);
                currentParent = newParent;
            }

            return currentParent;
        }

        public ILocalRoot CreateRoot(KeyLangReference key)
        {
            var builder = new ComponentBuilder();
            var component = builder.Ordered(false,
                builder.Value(null),
                builder.Value(key)
            );
            return new ConstructArgs(null, component.DetermainNext(null, true)[0], null);
        }

        public ArgsData GenerateNextArgsData(IParseArgs arg)
        {
            var node = new ValueBranchNode<IValueInfo<bool, Component>>(null);
            node.AddRange(
                arg.GetNextComponents()
                    .AsEnumerable()
                    .Select(data => new ValueBranchNode<IValueInfo<bool, Component>>(data))
            );

            node.Children.ForEach(n => ResolveNode(n));

            return new ArgsData(
                arg,
                node.Ends()
                    .Select(n => n.Path().Skip(1).Select(n => n.Value).ToList())
                    .ToList()
            );
        }
        /// <summary>
        /// Extend node as a construct with its own components and as group with its members.
        /// </summary>
        /// <param name="node">A node with children.</param>
        private void ResolveNode(ValueBranchNode<IValueInfo<bool, Component>> node)
        {
            if (node == null || node.Value == null)
            {
                return;
            }

            var key = Collection.GetKey(node.Value.Value.Reference);

            if (key == null || key is Token)
            {
                return;
            }
            else if (key is Construct)
            {
                // Get components from the construct.
                var components = (key as Construct).Component.DetermainNext(null, true).ToArray();

                // Add the components to the tree.
                var childNodes = components.Select(d => new ValueBranchNode<IValueInfo<bool, Component>>(d)).ToArray();
                node.AddRange(childNodes);

                // Resolve child nodes.
                childNodes.ForEach(n => ResolveNode(n));
            }
            else if (key is KeyGroup)
            {
                var group = key as KeyGroup;

                // Add the components to the tree.
                var childNodes = group.Members.Select(m =>
                {
                    var component = new ValueInfo<bool, Component>(node.Value, new Component(m));
                    return new ValueBranchNode<IValueInfo<bool, Component>>(component);
                }).ToArray();
                node.AddRange(childNodes);

                // Resolve child nodes.
                childNodes.ForEach(n => ResolveNode(n));
            }
        }
    }
}
