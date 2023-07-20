using System;
using System.Collections.Generic;
using System.IO;
using CC.Lexing;
using CC.Parcing;
using System.Linq;
using BranchList;

namespace CC
{
    class Program
    {
        private static BuildableCompiler bc = new BuildableCompiler();

        static void Main(string[] args)
        {
            //MakeTokens();
            MakeCCompiler();

            var p = @"Resources\TextFile2.txt";
            var l = bc.Lex(File.ReadAllText(p));
            //var g = bc.GetGroups();
            //bc.IterationParcing(l, bc.Constructs, g);

            foreach(var b in l)
            {
                Console.WriteLine(b.Index);
                Console.WriteLine(b.Key + " : " + b.Value);
                Console.WriteLine(b.Index + b.Value.Length);
            }
        }

        static void MakeTokens()
        {
            var t = bc.Tokens;
            t.Add(new Token { Key = "lessThan", Pattern = "<" });
            t.Add(new Token { Key = "gratherThan", Pattern = ">" });
            t.Add(new Token { Key = "equals", Pattern = "=" });
            t.Add(new Token { Key = "text", Pattern = "\".*?[^\\\\]\"" });
            t.Add(new Token { Key = "dot", Pattern = @"\." });
            t.Add(new Token { Key = "slash", Pattern = "/" });
            Token temp = new Token { Key = "identifier", Pattern = @"[a-zA-Z]\w*" };
            temp.SubTokens.Add(new Token { Key = "keyword_compiler", Pattern = "Compiler" });
            temp.SubTokens.Add(new Token { Key = "keyword_token", Pattern = "Token" });
            temp.SubTokens.Add(new Token { Key = "keyword_construct", Pattern = "Construct" });
            temp.SubTokens.Add(new Token { Key = "keyword_condition", Pattern = "Condition" });
            temp.SubTokens.Add(new Token { Key = "keyword_part", Pattern = "Part" });
            temp.SubTokens.Add(new Token { Key = "keyword_content", Pattern = "Content" });
            t.Add(temp);
        }

        static void MakeCCompiler()
        {
            var t = bc.Tokens;
            t.Add(new Token { Key = "open_parentases", Pattern = @"\(" });
            t.Add(new Token { Key = "close_parentases", Pattern = @"\)" });
            t.Add(new Token { Key = "open_brace", Pattern = "{" });
            t.Add(new Token { Key = "close_brace", Pattern = "}" });

            t.Add(new Token { Key = "semicolon", Pattern = ";" });

            Token temp = new Token { Key = "identifier", Pattern = @"[a-zA-Z]\w*" };
            temp.SubTokens.Add(new Token { Key = "keyword_int", Pattern = "int" });
            temp.SubTokens.Add(new Token { Key = "keyword_return", Pattern = "return" });
            t.Add(temp);

            t.Add(new Token { Key = "integer_literal", Pattern = "[0-9]+" });


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
