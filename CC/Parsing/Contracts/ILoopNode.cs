using CC.Key.ComponentTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parsing.Contracts
{
    public interface ILoopNode : ILocalRoot
    {
        /// <summary>
        /// Get the Args that comes in the next loop.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ILocalRoot> GetNextLoopArgs();

        /// <summary>
        /// Get all args that make up the loop.
        /// </summary>
        /// <param name="loopArg">A Arg object that of a new loop.</param>
        /// <returns>A list of all roots of that loop.</returns>
        IList<ILocalRoot> GetLoopArgs(ILocalRoot loopArg);

        /// <summary>
        /// Try to add the looping path. If this object doesn't have a path it will take it.
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool TryAddPath(IReadOnlyList<IValueComponentData> path);
    }
}
