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
    [Flags]
    public enum ConstructFactoryStatus
    {
        None,
        Complete,
        Halted
    }

    public class ConstructFactory : IConstructFactory
    {
        public IConstruct Key { get; }

        private ValueBranchNode<ComponentArgs> _content { get; }
        private readonly KeyCollection Keys;

        public ConstructFactoryStatus Status { get; protected set; }

        public List<List<ComponentArgs>> Contents
        {
            get
            {
                return _content.All()
                    .Where(node => node.Value.IsEnd)
                    .Select(arg => arg.Path().Skip(1).Select(v => v.Value).ToList())
                    .OrderByDescending(list => list.Count())
                    .ToList();
            }
        }

        public ConstructFactory(IConstruct key, KeyCollection keyCollection)
        {
            Key = key;
            Keys = keyCollection;
            _content = new ValueBranchNode<ComponentArgs>(null);
            _content.AddRange(
                Key.Components
                    .GetNextComponents()
                    .Where(comp => comp != null)
                    .Select(comp => new ValueBranchNode<ComponentArgs>(new ComponentArgs(comp)))
            );
            Status = _content.Children.Count() > 0 ? ConstructFactoryStatus.None : ConstructFactoryStatus.Halted;
        }

        public List<ValueComponent> GetWantedComponents()
        {
            return _content.Ends()
                .Where(node => !node.Value.IsEnd)
                .Select(node => node.Value.Component)
                .ToList();
        }

        public bool TryUseBlock(IBlock block)
        {
            var ends = _content.Ends()
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
                    if (allNextComponents.Any(c => c == null)) // Null indicates a end point to the construct components.
                    {
                        node.Value.IsEnd = true;
                        Status |= ConstructFactoryStatus.Complete;
                    }

                    // ### Add new layer of components. ###
                    var nextComponents = allNextComponents.Where(c => c != null);
                    if (nextComponents.Count() <= 0) return false;
                    node.AddRange(
                        nextComponents.Select(c => new ValueBranchNode<ComponentArgs>(new ComponentArgs(c)))
                    );
                    return true;
                });
            
            if (activeEnds.Count() <= 0)
            {
                Status |= ConstructFactoryStatus.Halted;
            }

            // Remove dead ends
            ends.Where(end => !activeEnds.Contains(end) && !end.Value.IsEnd)
                .ForEach(end =>
                {
                    do
                    {
                        var parent = end.Parent;
                        if (parent == null || parent.Value.IsEnd) return;
                        parent.Remove(end);
                        end = parent;
                    } while (end.Children.Count() == 0);
                });

            return Status == ConstructFactoryStatus.Halted;
        }

        public IConstructBlock MakeBlock()
        {
            if (Status != ConstructFactoryStatus.Complete) throw new Exception("The content is not complete.");

            var contents = Contents;

            if (contents.Count() == 0) throw new Exception("Code didn't set (IsEnd in ComponentArgs)/(this.IsComplete in ConstructFactory) correctly");
            if (contents.Count() == 1) return MakeBlock(contents[0]);
            throw new Exception("There are multiple blocks posible. (Use the MakeBlock function with endIndex)"); // TODO: make exception for returning all the options.
        }
        public IConstructBlock MakeBlock(int endIndex)
        {
            if (Status != ConstructFactoryStatus.Complete) throw new Exception("The content is not complete.");

            var content = Contents.Where(c => c.Last().Block.EndIndex == endIndex);

            if (content.Count() == 0) throw new Exception($"No content exists that en with index {endIndex}");
            if (content.Count() > 1) throw new Exception("")
        }

        private IConstructBlock MakeBlock(List<ComponentArgs> path)
        {
            return new ConstructBlock(Key, path.Select(arg => arg.Block).ToList());
        }
    }
}
