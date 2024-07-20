using ConLine.ProcessPipeline;
using ConLine.Steps;

namespace ConCli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var process = AsyncMain(args);
            process.Wait();
        }

        static async Task AsyncMain(string[] args)
        {
            if (args.Length != 2) {
                Console.WriteLine("expected : [target language] [first file]");
            }

            var langConfigFile = new StreamReader(File.OpenRead(args[0]));

            var langConfigFileString = await langConfigFile.ReadToEndAsync();
            langConfigFile.Close();
            Console.Write(langConfigFileString);
        }

        static void CreatePipeline()
        {
            var pipeline = new ProcessPipeLine("file parser");

            pipeline.AddStep(new Input<string>("filePath"));

            // single file no imports:
            // read file
            // lex and parse file
            // output abstract syntax tree

            // imports no loops:
            // read, lex and parse file
            // find imported file
            // * foreach import go back to step 1
            // add linkes to parsed imported files
            // output list of abstract syntax trees

            // imports:
            // read, lex and parse file
            // find imported file
            // * foreach import go back to step 1 if it hasn't been parsed before
            // add linkes to parsed imported files
            // output list of abstract syntax trees

            // TODO: Make a pipeline type that can use use storage
            // TODO: Make some form of general in pipeline storage
        }
    }
}