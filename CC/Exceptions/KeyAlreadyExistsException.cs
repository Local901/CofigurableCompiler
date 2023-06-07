using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Grouping
{
    public class KeyAlreadyExistsException : Exception
    {
        public KeyAlreadyExistsException(string key)
            : base($"Key already exists: {key}")
        {
        }
    }
}
