using CC.Contract;
using CC.Grouping;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CC.Parcing
{
    public class ComponentArgs : ParseArgs
    {
        public ComponentArgs(ValueComponent component, ILocalRoot localRoot)
            : base(component, localRoot)
        { }
    }
}
