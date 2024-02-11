using CC.Blocks;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Parsing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parsing
{
    public class LoopArgs : ConstructArgs, ILoopNode
    {
        private readonly IParseArgFactory factory;
        private List<ILocalRoot> loopPoints = new List<ILocalRoot>();

        public LoopArgs(Construct key, IValueComponentData component, ILocalRoot localRoot, IParseArgFactory factory)
            : base(key, component, localRoot)
        {
            this.factory = factory;
        }

        public IReadOnlyList<ILocalRoot> GetNextLoopArgs()
        {
            return loopPoints;
        }

        public IList<ILocalRoot> GetLoopArgs(ILocalRoot loopArg)
        {
            if (!loopPoints.Contains(loopArg))
            {
                throw new Exception("Provided loopArg is not the start of a new loop.");
            }

            var loopPath = new List<ILocalRoot> { loopArg };
            var currentArg = loopArg;
            while (currentArg != this)
            {
                currentArg = currentArg.LocalRoot;
                loopPath.Add(currentArg);
            }

            return loopPath.Reverse<ILocalRoot>().ToList();
        }

        public bool TryAddPath(IReadOnlyList<IValueComponentData> path)
        {
            if (path.Count == 0) return false;

            // Find the loop without first looped object.
            bool loopFound = false;
            var lastPathReference = path.Last().Component.Reference;
            var loopList = path.SkipWhile((data) =>
            {
                if (loopFound) return true;

                if (data.Component.Reference == lastPathReference)
                {
                    loopFound = true;
                }

                return false;
            }).ToList();

            // No loop found.
            if (loopList.Count == 0) return false;

            var loopReferenceList = loopList.Select((data) => data.Component.Reference).ToList();

            // Loop should contain this in its loop.
            int thisLoopIndex = loopReferenceList.IndexOf(Key.Reference);
            if (thisLoopIndex < 0) return false;

            // One of the known loops should contain the last element of the new loop with correct spacing.
            int loopDistance = loopReferenceList.Count - 1 - thisLoopIndex;
            bool containsThis = false;
            if (!loopPoints.Any((point) =>
            {
                var loopArgs = GetLoopArgs(point)
                    .Select((arg) => arg.Data.Component.Reference)
                    .ToList();
                var index = loopArgs.IndexOf(lastPathReference);

                if (index < 0) return false;

                containsThis = true;
                // Are they spaced the same distance appart.
                if (loopDistance != index) return false;

                // Check if items inbetween are the same.
                var inbetweenArgs = loopArgs.TakeWhile((arg, i) => 0 < i && i < index).ToList();
                for(int i = 0; i < inbetweenArgs.Count; i++)
                {
                    if (loopReferenceList[thisLoopIndex + i + 1] != inbetweenArgs[i])
                    {
                        return false;
                    }
                }
                return true;
            }))
            {
                // It is a loop for this object, but is already known so doesn't need to be added.
                return true;
            }

            // Don't add when it is not compairable.
            if (!containsThis) return false;

            // Add new loop.
            loopPoints.Add(factory.CreateNextArgs(path, this, null) as ILocalRoot);
            return true;
        }
    }
}
