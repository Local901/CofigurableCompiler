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
    public class ParseFactory : IParseFactory, IParseArgFactory
    {
        protected readonly KeyCollection Keys;
        protected readonly IParseArgFactory ArgsFactory;

        public readonly ILocalRoot ParseTree;
        private IReadOnlyList<IParseArgs> Ends;
        private Dictionary<IParseArgs, ArgsData> NextArgsData = new Dictionary<IParseArgs, ArgsData>();

        public ConstructBlock LastCompletion { get; private set; }

        public ParseFactory(IConstruct startConstruct, KeyCollection keys, IParseArgFactory argsFactory = null)
        {
            Keys = keys;
            ArgsFactory = argsFactory == null ? this : argsFactory;
            ParseTree = argsFactory.CreateNextArgs(startConstruct);
            ParseTree.ConstructCreated += (block) => LastCompletion = block;
        }

        public List<KeyLangReference> GetNextKeys()
        {
            return Ends.SelectMany(e => GetNextDataOfArgs(e)
                    .DataPaths
                    .Select(p => p.Last()))
                .Select(data => data.Component.Reference)
                .Distinct()
                .Where(data => data != null)
                .ToList();
        }

        public void UseBlock(IBlock block)
        {
            Ends = Ends.SelectMany(arg => NextArgs(arg, block)).ToList();

            var roots = Ends.GroupBy(arg => arg.LocalRoot)
                .OrderByDescending(group => group.Key.Depth)
                .Select(group => group.Key);

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

        /// <summary>
        /// Create the next args that can use the provided block.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        protected virtual IReadOnlyList<IParseArgs> NextArgs(IParseArgs arg, IBlock block)
        {
            if (block == null) throw new Exception("A block is required");

            var nextData = GetNextDataOfArgs(arg);
            var acceptable = nextData.DataPaths.Where(data =>
            {
                var reference = data.Last().Component.Reference;
                return Keys.GetLanguage(reference.Lang)
                    .IsKeyInGroup(block.Key.Reference, reference);
            }).ToList();

            if (acceptable.Count() == 0)
            {
                // TODO: make error args.
                return new List<IParseArgs>();
            }

            return acceptable.Select(data => ArgsFactory.CreateNextArgs(data, arg, block)).ToList();
        }

        public ArgsData GetNextDataOfArgs(IParseArgs arg)
        {
            if (NextArgsData.ContainsKey(arg))
            {
                return NextArgsData[arg];
            }
            var data = GenerateArgsData(arg);
            NextArgsData[arg] = data;
            return data;
        }
        /// <summary>
        /// Generate the ArgsData with the dataPaths to next ValueComponents
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual ArgsData GenerateArgsData(IParseArgs arg)
        {
            var node = new ValueBranchNode<IValueComponentData>(null);
            node.AddRange(
                arg.GetNextComponents()
                    .AsEnumerable()
                    .Select(data => new ValueBranchNode<IValueComponentData>(data))
            );

            ExtendConstructNodes(node);

            return new ArgsData(
                arg,
                node.Ends()
                    .Select(n => n.Path().Skip(1).Select(n => n.Value).ToList())
                    .ToList()
            );
        }
        private void ExtendConstructNodes(ValueBranchNode<IValueComponentData> node)
        {
            node.Ends()
                .Where(n => n.Value != null)
                .Select(n => {
                    var reference = n.Value.Component.Reference;
                    return new
                    {
                        node = n,
                        constructs = Keys.GetLanguage(reference.Lang).GetAllProminentSubKeys<IConstruct>(reference.Key, true)
                    };
                })
                .Where(n => n.constructs.Count > 0)
                .ForEach(n =>
                {
                    n.constructs.ForEach(construct =>
                    {
                        var data = construct.Component.GetNextComponents(null).ToArray();
                        // TODO: Check for loops.

                        n.node.AddRange(data.Select(d => new ValueBranchNode<IValueComponentData>(d)));
                    });

                    ExtendConstructNodes(n.node);
                });

        }

        public IParseArgs CreateNextArgs(IReadOnlyList<IValueComponentData> componentPath, IParseArgs parent, IBlock block)
        {
            if (componentPath == null || componentPath.Count == 0) throw new ArgumentException("ComponentPath should contain at leats one element.");

            var p = parent;
            foreach(var step in componentPath)
            {
                var localRoot = p is ILocalRoot ? (ILocalRoot)p : p.LocalRoot;
                IKey key = Keys.GetKey(step.Component.Reference);
                if (key is Token)
                {
                    // Should be last element in list.
                    p = new ParseArgs(step, localRoot, block);
                }
                else if (key is IConstruct)
                {
                    p = new ConstructArgs((IConstruct)key, step, localRoot);
                }
            }

            return p;
        }
    }
}
