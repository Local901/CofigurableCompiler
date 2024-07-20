using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public struct RunOptions
    {
        /// <summary>
        /// Have the pipeline run syncronously. (Helpful for debugging.)
        /// </summary>
        public bool RunSyncronous = false;

        public RunOptions() { }
    }
}
