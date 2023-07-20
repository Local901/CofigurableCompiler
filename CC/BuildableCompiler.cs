using CC.Contract;
using CC.Grouping;
using CC.Lexing;
using CC.Parcing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CC
{
    public class BuildableCompiler
    {
        public List<Token> Tokens { get; }
        public FileLexer FileLexer { get; }
        public List<Construct> Constructs { get; }
        public List<KeyGroup> Groups { get; }

        public BuildableCompiler()
        {
            Tokens = new List<Token>();
            Constructs = new List<Construct>();
            Groups = new List<KeyGroup>();
        }

        public string Compile(string code)
        {
            // Lexing
            var program = Lex(code);


            // Make Groups
            KeyCollection g = GetGroups();

            // Parcing
            //IterationParcing(program, Constructs, g);

            // string output
            //return stringOutput(program);
            return "";
        }

        public List<IBlock> Lex(string code)
        {
            var keys = GetGroups();
            FileLexer fileLexer = new FileLexer(code, keys);
            return Lex(fileLexer, keys.GetAllKeysOfType<Token>());
        }
        public static List<IBlock> Lex(FileLexer fileLexer, List<Token> tokens)
        {
            var program = new List<IBlock>();
            while(fileLexer.TryNextBlock(out IBlock block, tokens))
            {
                program.Add(block);
            }
            return program;
        }
        /*
        public void IterationParcing(Block program, IEnumerable<Construct> constructs, KeyCollection keys)
        {
            int changes = 1;
            int activeTriggers = 0;
            IEnumerable<Block> result = program.Content;
            while (changes > 0)
            {
                changes = 0;
                List<Block> temp = new List<Block>();
                List<Block> unusedBlocks = new List<Block>();
                List<ActiveConstruct> active = new List<ActiveConstruct>(); 

                foreach(Block b in result)
                {
                    unusedBlocks.Add(b);

                    // Try validate next Block
                    active = active.Where(a => a.TryValidate(b, groups)).ToList();

                    // find new active constructs
                    var newActive = constructs.Select(c => new ActiveConstruct { Construct = c })
                        .Where(a => a.TryValidate(b, groups))
                        .ToList();
                    activeTriggers += newActive.Count();

                    // add new active constructs to the others
                    active.AddRange(newActive);

                    // place unused blocks in temp is ther are no active constructs
                    if (active.Count() == 0)
                    {
                        temp.AddRange(unusedBlocks);
                        unusedBlocks.Clear();
                    }

                    // if an active construct is complete: Make block
                    //      -> add to temp
                    //      -> discard unused blocks
                    //      -> empty active list
                    var a = active.FirstOrDefault(a => a.Construct.Filters.Count == a.Index);
                    if (a != null)
                    {
                        temp.Add(a.MakeBlock());
                        unusedBlocks.Clear();
                        active.Clear();
                        changes++;
                    }
                }

                // place unused in temp
                temp.AddRange(unusedBlocks);
                unusedBlocks.Clear();

                // place temp in result
                result = temp;
            }

            program.Content.Clear();
            program.Content.AddRange(result);
            program.Content.ForEach(b => b.Parent = program);
        }
        */

        public KeyCollection GetGroups()
        {
            // TODO: handle exceptions if there are dubble keys!!!
            var groups = new KeyCollection();

            Tokens.ForEach(t => groups.AddKey(t));
            Constructs.ForEach(c => groups.AddKey(c));
            Groups.ForEach(g => groups.AddKey(g));

            return groups;
        }
    }
}
