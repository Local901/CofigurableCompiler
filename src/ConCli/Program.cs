using ConCli.Common;
using ConCli.Steps;
using ConCore.Blocks;
using ConCore.Key.Collections;
using ConLine;
using ConLine.Options;
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
            Console.WriteLine("Start program");

            var pipeline = CreatePipeline();

            await pipeline.Run(
                new RunOptions() { RunSyncronous = false },
                new CompleteInputOptions(
                    null,
                    new StepValue[]
                    {
                        new StepValue<PathInstance>("filePath", (PathInstance)args[1])
                    },
                    null,
                    null
                )
            );
        }

        static Pipeline CreatePipeline()
        {
            var pipeline = new ProcessPipeline("file parser");

            pipeline.Injector.AddSingleton<KeyCollection>();

            // Configure inputs
            pipeline.AddStep(new Input<PathInstance>("filePath"));

            // Configure language loader
            pipeline.AddStep(new LanguageLoaderStep("langLoader"));
            pipeline.AddConnection(new Connection("filePath", "filePath"), new Connection("langLoader", "path"));

            // Configure file parser
            pipeline.AddStep(new FileParserStep("fileParser"));
            pipeline.AddConnection(new Connection("filePath", "filePath"), new Connection("fileParser", "path"));
            pipeline.AddConnection(new Connection("langLoader", "result"), new Connection("fileParser", "langCollection"));

            // Print abstract syntay tree
            pipeline.AddStep(new PrintAbstractSyntaxTreeStep("print"));
            pipeline.AddConnection(new Connection("fileParser", "result"), new Connection("print", "block"));

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

            // TODO: Make some form of general in pipeline storage

            return pipeline;
        }
    }
}