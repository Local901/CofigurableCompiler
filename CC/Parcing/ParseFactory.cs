using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using BranchList;

namespace CC.Parcing
{
    public class ParseFactory : IParseFactory
    {
        private readonly KeyCollection Keys;
        private readonly IParseArgFactory ArgsFactory;
        private readonly IParseArgs ParseTree;

        public ParseFactory(IConstruct startConstruct, KeyCollection keys, IParseArgFactory argsFactory = null)
        {
            Keys = keys;
            ArgsFactory = argsFactory ?? new ParseArgFactory(keys);
            ParseTree = ArgsFactory.CreateArg(startConstruct);
        }

        public List<ValueComponent> GetNextKeys()
        {
            return ParseTree.Ends().Select(arg => arg.Component).ToList();
        }

        public void UseBlock(Block block)
        {
            var roots = ParseTree.Ends().AsEnumerable()
                .ForEach(arg => arg.UseBlock(block, Keys, ArgsFactory))
                .GroupBy(arg => arg.LocalRoot)
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

                var completeArgs = ends.Where(arg => arg.Status.HasFlag(ParseStatus.CanEnd));
                var activeArgs = ends.Where(arg => (arg.Status & ParseStatus.ReachedEnd) == 0);

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
                var deadEnds = ends.Where(arg => arg.Status.HasFlag(ParseStatus.ReachedEnd));
                deadEnds.ForEach(arg => arg.RemoveBranch());
            }
        }
    }
}
