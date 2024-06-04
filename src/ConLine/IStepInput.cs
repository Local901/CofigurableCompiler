using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public interface IStepInput
    {
        /// <summary>
        /// The values that have already been set.
        /// </summary>
        public IReadOnlyList<IInputValue> Values { get; }

        /// <summary>
        /// Send a new result.
        /// </summary>
        /// <param name="resultName">The name of the output stream.</param>
        /// <param name="value">The value to be send.</param>
        public void SendResult(string resultName, object? value);
    }
}
