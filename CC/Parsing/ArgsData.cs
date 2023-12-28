using CC.Key.ComponentTypes;
using CC.Parsing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC.Parsing
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
