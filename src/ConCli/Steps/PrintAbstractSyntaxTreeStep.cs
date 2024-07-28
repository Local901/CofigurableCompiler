using ConCore.Blocks;
using ConLine.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCli.Steps
{
    public delegate void PrintSyntaxTree(IBlock block);

    public class PrintAbstractSyntaxTreeStep : FunctionCallStep<PrintSyntaxTree>
    {
        public PrintAbstractSyntaxTreeStep(string name)
            : base(name)
        {
            FunctionHandler = (block) => PrintBlock(block);
        }

        private void PrintBlock(IBlock block, int depth = -1)
        {
            if (block is IRelationBlock relBlock)
            {
                PrintRelationBlock(relBlock, depth + 1);
            } else if (block is IValueBlock valueBlock)
            {
                for (int i = 0; i < depth; i++)
                {
                    Console.Write("  ");
                }
                Console.WriteLine($"{valueBlock.Value}\t\t{valueBlock.Key}");
            }
        }

        private void PrintRelationBlock(IRelationBlock relBlock, int depth)
        {
            foreach (IBlock c in relBlock.Content)
            {
                PrintBlock(c, depth);
            }
        }
    }
}
