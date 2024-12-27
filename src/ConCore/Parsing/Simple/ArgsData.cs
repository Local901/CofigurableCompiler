using ConCore.CustomRegex.Info;
using ConCore.Key.Components;
using ConCore.Parsing.Simple.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Parsing.Simple
{
    public struct ArgsData
    {
        public IParseArgs Args { get; }
        public IReadOnlyList<IReadOnlyList<IValueInfo<bool, Component>>> DataPaths { get; }

        public ArgsData(IParseArgs args, IReadOnlyList<IReadOnlyList<IValueInfo<bool, Component>>> dataPaths)
        {
            Args = args;
            DataPaths = dataPaths;
        }

        public IEnumerable<IReadOnlyList<IValueInfo<bool, Component>>> GetContinuingPaths()
        {
            return DataPaths.Where(path => path.Last() != null);
        }
    }
}
