using System;
using System.Collections.Generic;
using System.IO;
using BranchList;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Blocks;
using System.Linq;
using CC.Tools;
using CC.Parsing;

namespace CC
{
    class Program
    {
        private static KeyCollection keyCollection = new KeyCollection();

        static void Main(string[] args)
        {
            //MakeTokens();
            MakeCCompiler();

            var path = @"Resources\TextFile2.txt";
            var file = File.ReadAllText(path);
            var lexer = new FileLexer(file, keyCollection);
            var parser = new FileParser(lexer, new ParseArgFactory(keyCollection), keyCollection);

            /*var blocks = new List<IBlock>();
            IBlock block;
            var keys = keyCollection.GetLanguage("cLang").GetAllKeys().Select(k => k.Reference);
            while (lexer.TryNextBlock(out block, keys))
            {
                blocks.Add(block);
            }

            foreach(var b in blocks)
            {
                Console.WriteLine(b.Index);
                Console.WriteLine(b.Key + " : " + b.Value);
                Console.WriteLine(b.Index + b.Value.Length);
            }*/
            
            IBlock block = parser.DoParse(keyCollection.GetLanguage("cLang").GetKey("function").Reference);
            PrintConstruct(block);
        }

        static void PrintConstruct(IBlock block, string offSet = "")
        {
            if (block == null)
            {
                Console.WriteLine($"{offSet}Block was null.");
                return;
            }
            Console.WriteLine($"{offSet}{block.Index}");
            if (block is IValueBlock)
            {
                var vb = block as IValueBlock;
                Console.WriteLine($"{offSet}{block.Key} : {block.Name} : {vb.Value}");
            }
            else if (block is IRelationBlock)
            {
                var rb = block as IRelationBlock;
                Console.WriteLine($"{offSet}{block.Key} : {block.Name}[");
                rb.Content.ForEach(b => PrintConstruct(b, offSet + "  "));
                Console.WriteLine($"{offSet}]");
            }
            Console.WriteLine($"{offSet}{block.EndIndex}");
        }

        static void MakeCCompiler()
        {
            var lang = new LangCollection("cLang");
            keyCollection.AddLanguage(lang);

            lang.Add(new Token ("open_parentases", @"\("));
            lang.Add(new Token ("close_parentases", @"\)"));
            lang.Add(new Token ("open_brace", "{"));
            lang.Add(new Token ("close_brace", "}"));

            lang.Add(new Token ("semicolon", ";"));

            var identifier = lang.Add(new Token ("identifier", @"[a-zA-Z]\w*"));

            lang.Add(new Token ("integer_literal", "[0-9]+"));

            var c = new Construct("function",
                new OrderComponent(new List<IComponent>{
                    new ValueComponent(identifier, "return_type"),
                    new ValueComponent(identifier, "function_name"),
                    new ValueComponent(lang.CreateReference("open_parentases")),
                    new ValueComponent(lang.CreateReference("close_parentases")),
                    new ValueComponent(lang.CreateReference("open_brace")),
                    new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(identifier),
                        new ValueComponent(lang.CreateReference("integer_literal"), "return_value"),
                        new ValueComponent(lang.CreateReference("semicolon"))
                    }),
                    new ValueComponent(lang.CreateReference("close_brace"))
                }));

            lang.Add(c);

            /*var c = bc.Constructs;
            var cont = new Construct { Key = "function" };
            cont.Filters.Add(new Filter { Key = "keyword_int", Name = "return_type" });
            cont.Filters.Add(new Filter { Key = "identifier", Name = "function_name" });
            cont.Filters.Add(new Filter { Key = "open_parentases" });
            cont.Filters.Add(new Filter { Key = "close_parentases" });
            cont.Filters.Add(new Filter { Key = "open_brace" });
            cont.Filters.Add(new Filter { Key = "keyword_return" });
            cont.Filters.Add(new Filter { Key = "integer_literal" });
            cont.Filters.Add(new Filter { Key = "semicolon" });
            cont.Filters.Add(new Filter { Key = "close_brace" });
            c.Add(cont);

            cont = new Construct { Key = "return_int" };
            cont.Filters.Add(new Filter { Key = "keyword_return" });
            cont.Filters.Add(new Filter { Key = "integer_literal", Name = "int_value" });
            cont.Filters.Add(new Filter { Key = "semicolon" });
            */
        }
    }
}
