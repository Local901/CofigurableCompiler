using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Lexing.Errors
{
    public class ReadException : Exception
    {
        public static readonly string DEFAULT_MESSAGE = "Failed to read.";

        public ReadException()
            : this(DEFAULT_MESSAGE) { }
        public ReadException(string? message)
            : base(message ?? DEFAULT_MESSAGE) { }
    }
}
