using System;
using System.Collections.Generic;
using System.IO;
using BranchList;
using CC.Key;
using CC.Key.ComponentTypes;
using CC.Blocks;

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
            var parser = new FileParcer(lexer, keyCollection);

            /*var blocks = new List<IBlock>();
            IBlock block;
            while (lexer.TryNextBlock(out block))
            {
                blocks.Add(block);
            }

            foreach(var b in blocks)
            {
                Console.WriteLine(b.Index);
                Console.WriteLine(b.Key + " : " + b.Value);
                Console.WriteLine(b.Index + b.Value.Length);
            }*/

            ConstructBlock block;
            parser.DoParse(out block, keyCollection.GetLanguage("cLang").GetKey("function") as IConstruct);
            PrintConstruct(block);
        }

        static void PrintConstruct(ConstructBlock block, string offSet = "")
        {
            if (block == null)
            {
                Console.WriteLine($"{offSet}Block was null.");
                return;
            }
            Console.WriteLine($"{offSet}{block.Index}");
            if (block.Value != null)
            {
                Console.WriteLine($"{offSet}{block.Key} : {block.Name} : {block.Value}");
            }
            else
            {
                Console.WriteLine($"{offSet}{block.Key} : {block.Name}[");
                block.Content.ForEach(b => PrintConstruct(b, offSet + "  "));
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

            var identifier = lang.Add(new Token ("identifier", @"[a-zA-Z]\w*", new List<Token>
            {
                new Token ("keyword_int", "int"),
                new Token ("keyword_return", "return")
            }));

            lang.Add(new Token ("integer_literal", "[0-9]+"));

            var c = new Construct("function",
                new OrderComponent(new List<IComponent>{
                    new ValueComponent(lang.CreateReference("keyword_int"), "return_type"),
                    new ValueComponent(identifier, "function_name"),
                    new ValueComponent(lang.CreateReference("open_parentases")),
                    new ValueComponent(lang.CreateReference("close_parentases")),
                    new ValueComponent(lang.CreateReference("open_brace")),
                    new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent(lang.CreateReference("keyword_return")),
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
