using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Options
{
    internal interface OutputOptions : InputOptions
    {
        /// <summary>
        /// Add value to the output.
        /// </summary>
        /// <typeparam name="TType">Type of the values in the array.</typeparam>
        /// <param name="values">Values to add to the output.</param>
        public void AddOuputValue<TType>(IEnumerable<TType> values);
        /// <summary>
        /// Add a value to the output.
        /// </summary>
        /// <typeparam name="TType">Type of the value.</typeparam>
        /// <param name="value">Value to add to the output.</param>
        public void AddOutputValue<TType>(TType value);
    }
}
