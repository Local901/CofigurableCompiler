using CC.Blocks;

namespace CC
{
    public interface IMultiFileParser
    {
        /// <summary>
        /// Parse the file and return all parsed file information.
        /// </summary>
        /// <param name="filePaths">List of starter file paths.</param>
        /// <param name="configPaths">List configuration file paths</param>
        /// <returns>list of parsed files.</returns>
        public IBlock[] Parse(string[] filePaths, string[] configPaths);
    }
}
