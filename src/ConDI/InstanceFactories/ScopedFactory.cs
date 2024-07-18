using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConDI.InstanceFactories
{
    public class ScopedFactory<TInstance> : InstanceFactory<TInstance>
    {
        public override TInstance? CreateInstance(IScope scope)
        {
            return scope.CreateInstance<TInstance>();
        }
    }
}
