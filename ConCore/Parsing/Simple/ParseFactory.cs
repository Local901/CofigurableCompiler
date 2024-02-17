using BranchList;
using ConCore.Blocks;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Parsing.Simple.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConCore.Parsing.Simple
{
    public class ParseFactory : IParseFactory
    {
        // External tools.
        protected readonly KeyCollection Keys;
        protected readonly IParseArgFactory ArgsFactory;

        // Factory state data.
        public readonly ILocalRoot ParseTree;
        private IReadOnlyList<IParseArgs> Ends;
        private Dictionary<IParseArgs, ArgsData> NextArgsData = new Dictionary<IParseArgs, ArgsData>();

        // Result data
        public int NumberOfRounds { get; private set; }
        private List<IParseCompletion> CompletedParsings = new List<IParseCompletion>();
        public IReadOnlyList<IParseCompletion> Completed { get => CompletedParsings; }

        public ParseFactory(KeyLangReference startConstruct, KeyCollection keys, IParseArgFactory argsFactory)
        {
            NumberOfRounds = 0;
            Keys = keys;
            ArgsFactory = argsFactory;
            ParseTree = ArgsFactory.CreateRoot(startConstruct);
            Ends = new List<IParseArgs> { ParseTree };
        }

        public List<KeyLangReference> GetNextKeys()
        {
            return Ends.SelectMany(e => GetNextDataOfArgs(e)
                    .DataPaths
                    .Select(p => p.Last())
                    .Where(p => p != null))
                .Select(data => data.Component.Reference)
                .Distinct()
                .Where(data => data != null)
                .ToList();
        }

        public void UseBlocks(IEnumerable<IBlock> blocks)
        {
            NumberOfRounds++;

            // Find next accepted ParseArgs.
            var acceptedNextArgs = blocks.SelectMany((block) => PropogateBlock(Ends, block)).ToArray();

            // Update the status of the args.
            acceptedNextArgs.ForEach((arg) => UpdateStatus(arg));

            // Clear cache of next arg data.
            NextArgsData.Clear();

            // Resove the args based on the status
            var resovedArgs = acceptedNextArgs.SelectMany((arg) => ResolveStatus(arg)).ToArray();

            // Remove branches that have reached the end, and arn't direct children of the root of the parse tree.
            resovedArgs.Where((arg) => arg.Status.HasFlag(ParseStatus.ReachedEnd) && arg.Parent != ParseTree)
                .ForEach((arg) => arg.RemoveBranch());

            Ends = resovedArgs.Where((arg) => !arg.Status.HasFlag(ParseStatus.ReachedEnd)).ToList();

            // Add blocks that have propogated to the root to the result list.
            ProcessCompleted();
        }

        /// <summary>
        /// Apply block to the continuation of the args and return the values that have succesfuly been propogated.
        /// </summary>
        /// <param name="args">The list of args to start from.</param>
        /// <param name="block">The block to propogate with.</param>
        /// <returns>Values that have been successfuly propogated.</returns>
        protected virtual IEnumerable<IParseArgs> PropogateBlock(IEnumerable<IParseArgs> args, IBlock block)
        {
            return args.SelectMany((arg) =>
            {
                // Get all data related to the propegation of a arg.
                var argData = GetNextDataOfArgs(arg);

                // Find all paths that can be used with the block.
                var paths = argData.GetContinuingPaths()
                    .Where(path => Keys.IsKeyInGroup(block.Key, path.Last().Component.Reference))
                    .ToArray();

                return paths.Select((path) => ArgsFactory.CreateNextArgs(path, arg, block));
            });
        }

        public ArgsData GetNextDataOfArgs(IParseArgs arg)
        {
            if (NextArgsData.ContainsKey(arg))
            {
                return NextArgsData[arg];
            }
            var data = ArgsFactory.GenerateNextArgsData(arg);
            NextArgsData[arg] = data;
            return data;
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
        /// Resolve the arg following the status.
        /// </summary>
        /// <param name="arg">The arg to resolve.</param>
        /// <returns>A list of args created/found including arg itself.</returns>
        protected virtual IEnumerable<IParseArgs> ResolveStatus(IParseArgs arg)
        {
            var result = new List<IParseArgs> { arg };

            if (!arg.Status.HasFlag(ParseStatus.CanEnd) || arg.LocalRoot == null)
            {
                return result;
            }

            var localRoot = arg.LocalRoot;
            if (localRoot.Parent != null)
            {
                // Create block from ended branch.
                var block = localRoot.CreateBlock(arg);

                // Create, Update and resolve root arg with block.
                var rootArg = ArgsFactory.CreateNextArgs(new List<IValueComponentData> { localRoot.Data }, localRoot.Parent, block);
                UpdateStatus(rootArg);
                result.AddRange(ResolveStatus(rootArg));
            }

            return result;
        }

        /// <summary>
        /// Add compleded constructs to the ParseCollection.
        /// </summary>
        private void ProcessCompleted()
        {
            ParseTree.Children.Where(arg => arg.Block != null)
                .ForEach(arg =>
                {
                    CompletedParsings.Add(new ParseCompletion(NumberOfRounds, arg.Block));
                    arg.RemoveBranch();
                });
        }

        public void CompleteEnds()
        {
            Ends.ForEach(end =>
            {
                var arg = end;
                while (arg.LocalRoot != null)
                {
                    var localRoot = arg.LocalRoot;
                    var block = localRoot.CreateBlock(arg);
                    arg = ArgsFactory.CreateNextArgs(new List<IValueComponentData> { localRoot.Data }, localRoot.Parent, block);
                    UpdateStatus(arg);
                }
            });
            ProcessCompleted();
        }
    }
}
