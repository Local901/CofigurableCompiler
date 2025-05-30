using ConCli.Common;
using ConCore.CustomRegex;
using ConCore.FileInfo;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Key.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCli.Langs
{
    public class JsonLang
    {

        public static Language JsonLangConfigLoader(PathInstance path, KeyCollection keyCollection)
        {
            var lang = keyCollection.GetLanguage("json");
            if (lang != null)
            {
                return lang;
            }

            lang = new Language("json");

            var dubblequote_token = lang.AddKey(new Token("doublequote", "\""));
            var colon_token = lang.AddKey(new Token("colon", ":"));
            var comma_token = lang.AddKey(new Token("comma", ","));
            var brace_open = lang.AddKey(new Token("brace_open", @"\{"));
            var brace_close = lang.AddKey(new Token("brace_close", @"\}"));
            var block_open = lang.AddKey(new Token("block_open", @"\["));
            var block_close = lang.AddKey(new Token("block_close", @"\]"));
            var true_token = lang.AddKey(new Token("bool_true", "true"));
            var false_token = lang.AddKey(new Token("bool_false", "false"));
            var null_token = lang.AddKey(new Token("null", "null"));

            string integerPattern = "(([1-9][0-9]*)|0[^0-9]+)";
            var integerValue = lang.AddKey(new Token("integer", integerPattern));
            var decimalValue = lang.AddKey(new Token("decimal", @$"{integerPattern}\.[0-9]+"));
            var number = lang.AddKey(new Token("number", @$"{integerPattern}(\.[0-9]+)?([eE][\+-]?[0-9]+)?"));
            lang.SetAlias(number, integerValue);
            lang.SetAlias(number, decimalValue);

            // All boolean values
            var boolean_group_ref = lang.AddKey(new KeyGroup("boolean", new List<KeyLangReference>
            {
                true_token,
                false_token,
            }));

            // Group of all types of values.
            var valueType_group = new KeyGroup("value_types", new List<KeyLangReference>
            {
                number,
                boolean_group_ref,
                null_token,
            });
            var valueType_group_ref = lang.AddKey(valueType_group);

            var builder = new ComponentBuilder();
            var conBuilder = new ConditionBuilder();

            // A string.
            var string_construct = lang.AddKey(new Construct("encoded_string",
                builder.Ordered(false,
                    builder.Value(dubblequote_token),
                    // TODO: capture inbetween
                    builder.Value(
                        dubblequote_token,
                        new PrecedingOptions(
                            conBuilder.Regex("(?:[^\"\"\\\\]|\\\\(?:[\"\"\\\\/bfnrt]|u[0-9a-fA-F]{4}))*"),
                            true,
                            "value"
                        )
                    )
                )
            ));
            valueType_group.Add(string_construct);

            // A json object.
            var object_construct = lang.AddKey(new Construct("json_object",
                builder.Ordered(false,
                    builder.Value(brace_open),
                    builder.SeparatedList(
                        builder.Ordered(false,
                            builder.Value(string_construct),
                            builder.Value(colon_token),
                            builder.Value(valueType_group_ref)
                        ),
                        builder.Value(comma_token),
                        true,
                        SeparatorOptions.NEVER
                    ),
                    builder.Value(brace_close)
                )
            ));
            valueType_group.Add(object_construct);

            // An non type restricted list
            var list_construct = lang.AddKey(new Construct("json_list",
                builder.Ordered(false,
                    builder.Value(block_open),
                    builder.SeparatedList(
                        builder.Value(valueType_group_ref),
                        builder.Value(comma_token),
                        true,
                        SeparatorOptions.NEVER
                    ),
                    builder.Value(block_close)
                )
            ));
            valueType_group.Add(list_construct);

            lang.StartingKeyReference = valueType_group_ref;

            return lang;
        }
    }
}
