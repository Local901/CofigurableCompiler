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

            var activeEnds = ends.Where(node =>
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
            throw new NotImplementedException();
        }
    }
}
