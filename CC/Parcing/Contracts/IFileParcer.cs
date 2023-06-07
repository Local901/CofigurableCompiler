using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC.Parcing.Contracts
{
    public interface IFileParcer
    {
        void DoParse(out IBlock block, IConstruct startConstruct);
    }
}
