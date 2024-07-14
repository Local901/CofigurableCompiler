using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Options
{
    public interface MemoryOptions: InputOptions
    {
        /// <summary>
        /// Set a value in memory. This will only work if the step is an intance of Memory.
        /// </summary>
        /// <typeparam name="TType">Type of value.</typeparam>
        /// <param name="value">The value to set in the memeory.</param>
        public void SetMemory<TType>(TType value);

        /// <summary>
        /// Get a value from memory. This will only work if the step is an instance of memory.
        /// </summary>
        /// <typeparam name="TType">Type of the value</typeparam>
        /// <returns>The value stored in memeory</returns>
        public TType? GetMemory<TType>();
    }
}
