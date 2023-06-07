using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Exceptions
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string key)
            :base($"Key not found: {key}")
        { }
    }
}
