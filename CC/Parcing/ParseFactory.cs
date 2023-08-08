using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using BranchList;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Blocks;

namespace CC.Parcing
{
    public class ParseFactory : IParseFactory
    {
        private readonly KeyCollection Keys;
        private readonly IParseArgFactory ArgsFactory;
        public readonly ILocalRoot ParseTree;

        public ParseFactory(IConstruct startConstruct, KeyCollection keys, IParseArgFactory argsFactory = null)
        {
            Keys = keys;
            ArgsFactory = argsFactory ?? new ParseArgFactory(keys);
            ParseTree = ArgsFactory.CreateArg(startConstruct);
            ParseTree.ConstructCreated += (block) => LastCompletion = block;
        }

        public ConstructBlock LastCompletion { get; private set; }

        public List<ValueComponent> GetNextKeys()
        {
            return ParseTree.Ends().Select(arg => arg.Component).ToList();
        }

        public void UseBlock(IBlock block)
        {
            var endPoints = ParseTree.Ends()
                .Where(arg => arg.Status == ParseStatus.None)
                .ToList();

            endPoints.ForEach(arg => arg.UseBlock(block, Keys, ArgsFactory));

            var roots = endPoints.GroupBy(arg => arg.LocalRoot)
                .OrderByDescending(group =>
                {
                    int depth = 0;
                    ILocalRoot root = group.Key;
                    while(root != null)
                    {
                        depth++;
                        root = root.LocalRoot;
                    }
                    return depth;
                }).Select(group => group.Key);

            foreach (var root in roots)
            {
                if (root == null) continue;

                // Get potentionaly new end points.
                var ends = root.LocalEnds()
                    .Select(arg => {
                        // When a LocalRoot has already been used it should not be counted.
                        if (arg is ILocalRoot && !arg.Children.All(c => c.Status == ParseStatus.None))
                        {
                            return null;
                        }

                        return arg.Status != ParseStatus.None ? arg : arg.Parent;
                    })
                    .Distinct()
                    .Where(arg => arg != null);

                var completeArgs = ends.Where(arg => arg.Status.HasFlag(ParseStatus.CanEnd)).ToList();
                var activeArgs = ends.Where(arg => (arg.Status & ParseStatus.ReachedEnd) == 0).ToList();
                var errorArgs = ends.Where(arg => arg.Status.HasFlag(ParseStatus.Error)).ToList();

                if (completeArgs.Count() > 0)
                {
                    bool isCompleted = activeArgs.Count() == 0;

                    foreach(var arg in completeArgs.Skip(isCompleted ? 1 : 0))
                    {
                        root.SplitCompleteFrom(arg, Keys, ArgsFactory);
                    }
                    if (isCompleted)
                    {
                        root.CompleteFrom(completeArgs.First(), Keys, ArgsFactory);
                        continue;
                    }
                }

                // Remove deadEnds
                bool onlyErrors = errorArgs.Count() == ends.Count();
                // Find dead end nodes.
                var deadEnds = ends.Where(arg =>
                {
                    if (arg.Status.HasFlag(ParseStatus.ReachedEnd)) return true;

                    // Only remove error when there is a path that doesn't have a error.
                    if (!onlyErrors && arg.Status.HasFlag(ParseStatus.Error))
                    {   // Only remove error nodes that don't have any children that already have a status.
                        return arg.Children.Where(c => c.Status != ParseStatus.None).Count() == 0;
                    }
                    return false;
                });
                deadEnds.ForEach(arg => arg.RemoveBranch());
            }
        }
    }
}
