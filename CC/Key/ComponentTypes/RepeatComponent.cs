using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CC.Key.ComponentTypes
{
    /// <summary>
    /// Repeat child or continue with parent.
    /// </summary>
    public class RepeatComponent : IComponent
    {
        public RepeatComponent(IComponent child)
            : base()
        {
            if (child == null) throw new ArgumentNullException("Child should be a component.");
            if (child is RepeatComponent) throw new ArgumentException("The child of a repeatComponent can not be another repeatComponent");
            Add(child);
        }

        public override IList<ValueComponent> GetNextComponents()
        {
            var repeatResult = Children[0].GetValueComponents();
            return repeatResult.Concat(
                Parent == null
                    ? new List<ValueComponent> { null }
                    : Parent.GetValueComponents(this)
                ).ToList();
        }

        public override IList<ValueComponent> GetValueComponents(IComponent startAfter = null)
        {
            return GetNextComponents();
        }
    }
}
