using BranchList;
using CC.Contract;
using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    internal class ConstructFactory : IConstructFactory
    {
        public IConstruct Key { get; }

        private ValueBranchNode<ComponentArgs> Content { get; }
        private readonly KeyCollection Keys;

        public bool IsComplete { get; protected set; }

        public bool CanContinue { get; protected set; }

        public ConstructFactory(IConstruct key, KeyCollection keyCollection)
        {
            Key = key;
            Keys = keyCollection;
            Content = new ValueBranchNode<ComponentArgs>(null);
            Content.AddRange(
                Key.Components
                    .GetNextComponents()
                    .Where(comp => comp != null)
                    .Select(comp => new ValueBranchNode<ComponentArgs>(new ComponentArgs(comp)))
            );
            CanContinue = Content.Children.Count() > 0;
        }

        public List<ValueComponent> GetWantedComponents()
        {
            return Content.Ends()
                .Where(node => !node.Value.IsEnd)
                .Select(node => node.Value.Component)
                .ToList();
        }

        public bool TryUseBlock(IBlock block)
        {
            var ends = Content.Ends()
                .Where(node => !node.Value.IsEnd);

            var activeEnds = ends.Where(node => // return true if node can continue else false
                {
                    // ### Test if block is for component. ###
                    bool isRelated = Keys.IsKeyOfGroup(block.Key, node.Value.Component.Key);
                    if (!isRelated)
                    {
                        // TODO: Add Error block/ Try to resolve.
                        return false;
                    }

                    // ### If usable add next layer of components. ###
                    var allNextComponents = node.Value.Component.GetNextComponents();

                        // Can this node be used to complete the construct.
                    if (allNextComponents.Any(c => c == null))
                    {
                        node.Value.IsEnd = true;
                        IsComplete = true;
                    }

                    var nextComponents = allNextComponents.Where(c => c != null);

                    if (nextComponents.Count() <= 0) return false;
                    
                    node.AddRange(
                        nextComponents.Select(c => new ValueBranchNode<ComponentArgs>(new ComponentArgs(c)))
                    );
                    return true;
                });
            
            if (activeEnds.Count() <= 0)
            {
                CanContinue = false;
            }

            return CanContinue;
        }

        public IBlock MakeBlock()
        {
            if (!IsComplete) throw new Exception("The content is not complete.");

            var contents = Content.AllFirst(arg => arg.Value.IsEnd)
                .Select(arg => arg.Path().Skip(1).ToList())
                .OrderByDescending(list => list.Count())
                .ToList();

            if (contents.Count() == 0) throw new Exception("Code didn't set (IsEnd in ComponentArgs)/(this.IsComplete in ConstructFactory) correctly");
            if (contents.Count() == 1) return MakeBlock(contents[0]);
            if (contents[0].Count() == contents[1].Count()) throw new Exception("Multiple options (This should be a special exeption that passes the options)");
            return MakeBlock(contents[0]);
        }

        private IBlock MakeBlock(List<ValueBranchNode<ComponentArgs>> path)
        {
            var block = new Block
            {
                Key = Key,
                Content = path.Select(arg => arg.Value.Block).ToList(),
                Index = path.First().Value.Block.Index,
                EndIndex = path.Last().Value.Block.EndIndex
            };
            path.ForEach(arg => arg.Value.Block.Parent = block);
            return block;
        }
    }
}
