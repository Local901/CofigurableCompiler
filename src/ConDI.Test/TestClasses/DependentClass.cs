using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConDI.Test.TestClasses
{
    public class DependentClass
    {
        public EmptyClass Dependency { get; set; }
        public DependentClass(EmptyClass dependency)
        {
            Dependency = dependency;
        }
    }
}
