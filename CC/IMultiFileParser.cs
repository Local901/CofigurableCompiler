using CC.Blocks;

namespace CC
{
    public interface IMultiFileParser
    {
        public IBlock[] Parse(string filePath);
    }
}
