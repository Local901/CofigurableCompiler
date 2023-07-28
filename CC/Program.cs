using System;
using System.Collections.Generic;
using System.IO;
using CC.Lexing;
using CC.Parcing;
using System.Linq;
using BranchList;
using CC.Grouping;
using CC.Contract;
using CC.Parcing.ComponentTypes;
using CC.Parcing.Contracts;

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
            parser.DoParse(out block, keyCollection.GetKey("function") as IConstruct);
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

        static void MakeTokens()
        {
            keyCollection.AddKey(new Token { Key = "lessThan", Pattern = "<" });
            keyCollection.AddKey(new Token { Key = "gratherThan", Pattern = ">" });
            keyCollection.AddKey(new Token { Key = "equals", Pattern = "=" });
            keyCollection.AddKey(new Token { Key = "text", Pattern = "\".*?[^\\\\]\"" });
            keyCollection.AddKey(new Token { Key = "dot", Pattern = @"\." });
            keyCollection.AddKey(new Token { Key = "slash", Pattern = "/" });
            Token temp = new Token { Key = "identifier", Pattern = @"[a-zA-Z]\w*" };
            temp.SubTokens.Add(new Token { Key = "keyword_compiler", Pattern = "Compiler" });
            temp.SubTokens.Add(new Token { Key = "keyword_token", Pattern = "Token" });
            temp.SubTokens.Add(new Token { Key = "keyword_construct", Pattern = "Construct" });
            temp.SubTokens.Add(new Token { Key = "keyword_condition", Pattern = "Condition" });
            temp.SubTokens.Add(new Token { Key = "keyword_part", Pattern = "Part" });
            temp.SubTokens.Add(new Token { Key = "keyword_content", Pattern = "Content" });
            keyCollection.AddKey(temp);
        }

        static void MakeCCompiler()
        {
            keyCollection.AddKey(new Token { Key = "open_parentases", Pattern = @"\(" });
            keyCollection.AddKey(new Token { Key = "close_parentases", Pattern = @"\)" });
            keyCollection.AddKey(new Token { Key = "open_brace", Pattern = "{" });
            keyCollection.AddKey(new Token { Key = "close_brace", Pattern = "}" });

            keyCollection.AddKey(new Token { Key = "semicolon", Pattern = ";" });

            Token temp = new Token { Key = "identifier", Pattern = @"[a-zA-Z]\w*" };
            temp.SubTokens.Add(new Token { Key = "keyword_int", Pattern = "int" });
            temp.SubTokens.Add(new Token { Key = "keyword_return", Pattern = "return" });
            keyCollection.AddKey(temp);

            keyCollection.AddKey(new Token { Key = "integer_literal", Pattern = "[0-9]+" });

            var c = new Construct("function",
                new OrderComponent(new List<IComponent>{
                    new ValueComponent("keyword_int", "return_type"),
                    new ValueComponent("identifier", "function_name"),
                    new ValueComponent("open_parentases"),
                    new ValueComponent("close_parentases"),
                    new ValueComponent("open_brace"),
                    new OrderComponent(new List<IComponent>
                    {
                        new ValueComponent("keyword_return"),
                        new ValueComponent("integer_literal", "return_value"),
                        new ValueComponent("semicolon")
                    }),
                    new ValueComponent("close_brace")
                }));

            keyCollection.AddKey(c);

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
