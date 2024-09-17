using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Reading.Errors
{
    public class EndOfFileException : ReadException
    {
        public EndOfFileException()
            :base("End of file reached before reading a block.") { }
    }
}
