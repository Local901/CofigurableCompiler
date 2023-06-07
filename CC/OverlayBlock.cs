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
            set => _block.Key = value; 
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
            set => _block.Value = value;
        }
        public int Index
        {
            get => _block.Index;
            set => _block.Index = value;
        }
        public int EndIndex
        {
            get => _block.EndIndex;
            set => _block.EndIndex = value;
        }
        public List<IBlock> Content
        {
            get => _block.Content;
            set => _block.Content = value;
        }
        public IBlock Parent
        {
            get => _parent;
            set => _parent = value;
        }
        private IBlock _parent;

        public OverlayBlock(IBlock block)
        {
            _block = block;
        }
    }
}
