using ConCore.Key.Components;
using ConCore.Parsing.Simple.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ConCore.Parsing.Simple
{
    public struct ArgsData
    {
        public IParseArgs Args { get; }
        public IReadOnlyList<IReadOnlyList<IValueComponentData>> DataPaths { get; }

        public ArgsData(IParseArgs args, IReadOnlyList<IReadOnlyList<IValueComponentData>> dataPaths)
        {
            Args = args;
            DataPaths = dataPaths;
        }

        public IEnumerable<IReadOnlyList<IValueComponentData>> GetContinuingPaths()
        {
            return DataPaths.Where(path => path.Last() != null);
        }
    }
}
