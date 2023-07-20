using CC.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC
{
    public class OverlayBlock : IBlock
    {
        private IBlock _block;

        public IKey Key 
        { 
            get => _block.Key;
        }
        public string Name 
        {
            get => _name;
            set => _name = value;
        }
        private string _name = "";
        public string Value
        {
            get => _block.Value;
        }
        public int Index
        {
            get => _block.Index;
        }
        public int EndIndex
        {
            get => _block.EndIndex;
        }

        public OverlayBlock(IBlock block)
        {
            _block = block;
        }
    }
}
