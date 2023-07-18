using CC.Contract;
using CC.Parcing.ComponentTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CC.Parcing
{
    public class ComponentArgs
    {
        public ValueComponent Component { get; }
        public IBlock Block { get; set; }
        /// <summary>
        /// IsEnd helps to indecate if this can be the end of the input blocks. Has to be set by hand. Default false.
        /// </summary>
        public bool IsEnd { get; set; }

        public ComponentArgs(ValueComponent component, IBlock block)
        {
            Component = component;
            Block = block;
        }
        public ComponentArgs(ValueComponent component)
        {
            Component = component;
        }
    }
}
