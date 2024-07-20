using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConDI.InstanceFactories
{
    public delegate TInstance InstanceGenerator<TInstance>(IScope scope);

    public class FunctionFactory<TInstance> : InstanceFactory<TInstance>
    {
        private readonly InstanceGenerator<TInstance> _instanceGenerator;

        public FunctionFactory(InstanceGenerator<TInstance> instanceGenerator)
        {
            _instanceGenerator = instanceGenerator;
        }

        public override TInstance? CreateInstance(IScope scope)
        {
            return _instanceGenerator(scope);
        }
    }
}
