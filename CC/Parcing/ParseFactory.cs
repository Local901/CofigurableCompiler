using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using BranchList;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Blocks;
using System.Data;

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
            var groups = Ends.SelectMany(arg => NextArgs(arg, block))
                .GroupBy(arg => arg.LocalRoot)
                .OrderByDescending(group => group.Key.Depth)
                .ToList();

            NextArgsData.Clear();

            groups.ForEach(g => g.ForEach(arg => UpdateStatus(arg)));
            var nextEnds = groups.SelectMany(group =>
            {
                var errors = group.Where(arg => arg.Status.HasFlag(ParseStatus.Error)).ToList();
                if (group.Count() == errors.Count)
                {
                    return group;
                }
                errors.ForEach(err => err.RemoveBranch());
                return group.Where(arg => !arg.Status.HasFlag(ParseStatus.Error));
            }).ToList();

            Ends = nextEnds.OrderByDescending(arg => arg.LocalRoot.Depth)
                .SelectMany(arg => UpdateEnd(arg))
                .ToList();
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

        /// <summary>
        /// Set the status of the provided arg.
        /// </summary>
        /// <param name="arg"></param>
        private void UpdateStatus(IParseArgs arg)
        {
            var data = GetNextDataOfArgs(arg);
            var ends = data.DataPaths.Select(path => path.Last());
            if (ends.Contains(null)) arg.Status |= ParseStatus.CanEnd;
            if (ends.Where(v => v != null).Count() == 0) arg.Status |= ParseStatus.ReachedEnd;
            if (arg.Block is ErrorBlock) arg.Status |= ParseStatus.Error;
        }

        /// <summary>
        /// Update the provided argument as if it is an end of the tree and return a list of all the ends resulting from the arg arg included if it is still an end.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>List of all ends resulting from the arg (may include self)</returns>
        public IReadOnlyList<IParseArgs> UpdateEnd(IParseArgs arg)
        {
            List<IParseArgs> result = new List<IParseArgs>();

            if (!(arg.Block is ErrorBlock))
            {
                if (!arg.Status.HasFlag(ParseStatus.ReachedEnd))
                {
                    result.Add(arg);
                }
                if (arg.Status.HasFlag(ParseStatus.CanEnd))
                {
                    var localRoot = arg.LocalRoot;
                    if (localRoot == null) return result;

                    var block = localRoot.CreateBlock(arg);
                    if (localRoot.Parent != null)
                    {
                        var rootArg = CreateNextArgs(new List<IValueComponentData> { localRoot.Data }, localRoot.Parent, block);
                        UpdateStatus(rootArg);
                        result.AddRange(UpdateEnd(rootArg));
                    }
                }
                return result;
            }
            throw new NotImplementedException("Updating the end when the block is an error.");
        }

        public IParseArgs CreateNextArgs(IReadOnlyList<IValueComponentData> componentPath, IParseArgs parent, IBlock block)
        {
            if (componentPath == null || componentPath.Count == 0) throw new ArgumentException("ComponentPath should contain at leats one element.");

            var p = parent;
            for (int i = 0; i < componentPath.Count; i++)
            {
                var step = componentPath[i];
                var localRoot = p is ILocalRoot ? (ILocalRoot)p : p.LocalRoot;
                IKey key = Keys.GetKey(step.Component.Reference);
                IParseArgs np = null;
                if (key is Token)
                {
                    // Should be last element in list.
                    np = new ParseArgs(step, localRoot, block);
                }
                else if (key is IConstruct)
                {
                    np = i == componentPath.Count - 1
                        ? new ConstructArgs((IConstruct)key, step, localRoot)
                        : new ConstructArgs((IConstruct)key, step, localRoot, block);
                }
                if (p != null)
                {
                    p.Add(np);
                }
                p = np;
            }

            return p;
        }
    }
}
