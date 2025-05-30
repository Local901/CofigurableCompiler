using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCore.Key.Conditions
{
    public class ConditionResponse
    {
        public int EndPoint { get; }

        public ConditionResponse(int endPoint)
        {
            EndPoint = endPoint;
        }
    }
}
