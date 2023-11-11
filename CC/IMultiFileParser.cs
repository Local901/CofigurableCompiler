using CC.FileInfo;

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
        public FileData[] Parse(string filePaths);
    }
}
