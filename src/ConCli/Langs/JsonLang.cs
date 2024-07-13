using ConCore.FileInfo;
using ConCore.Key;
using ConCore.Key.Collections;
using ConCore.Key.Components;
using ConCore.Key.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCli.Langs
{
    public class JsonLang
    {

        public static LangCollection JsonLangConfigLoader(FileData file, KeyCollection keyCollection)
        {
            var lang = keyCollection.GetLanguage("json");
            if (lang != null)
            {
                return lang;
            }

            lang = new LangCollection("json");

            var dubblequote_token = lang.Add(new Token("doublequote", "(?<!(\\\\)*\\)\""));
            var colon_token = lang.Add(new Token("colon", ":"));
            var comma_token = lang.Add(new Token("comma", ","));
            var brace_open = lang.Add(new Token("brace_open", @"\{"));
            var brace_close = lang.Add(new Token("brace_close", @"\}"));
            var block_open = lang.Add(new Token("block_open", @"\["));
            var block_close = lang.Add(new Token("block_close", @"\]"));
            var true_token = lang.Add(new Token("bool_true", "true"));
            var false_token = lang.Add(new Token("bool_false", "false"));
            var null_token = lang.Add(new Token("null", "null"));

            var integerValue = new Token("integer", "[([1-9][0-9]*)0]");
            var decimalValue = new Token("decimal", @$"{integerValue.Pattern}\.[0-9]+");
            var number = new Token("number", @$"{integerValue.Pattern}(\.[0-9]+)?([eE][\+-]?[0-9]+)?");
            number.AddAlias(integerValue);
            number.AddAlias(decimalValue);

            lang.Add(integerValue);
            lang.Add(decimalValue);

            // All boolean values
            var boolean_group_ref = lang.Add(new KeyGroup("boolean", new List<KeyLangReference>
            {
                true_token,
                false_token,
            }));

            // Group of all types of values.
            var valueType_group = new KeyGroup("value_types", new List<KeyLangReference>
            {
                lang.Add(number),
                boolean_group_ref,
                null_token,
            });
            var valueType_group_ref = lang.Add(valueType_group);

            // A string.
            var string_construct = lang.Add(new Construct("encoded_string", new OrderComponent(new List<IComponent>
            {
                new ValueComponent(dubblequote_token),
                // TODO: every thing in between (prefured without new lines).
                new ValueComponent(dubblequote_token),
            })));
            valueType_group.Add(string_construct);

            // A json object.
            var object_construct = lang.Add(new Construct("json_object", new OrderComponent(new List<IComponent>
            {
                new ValueComponent(brace_open),
                new SpacedRepeatComponent(
                    new ValueComponent(comma_token),
                    new OrderComponent(new List<IComponent> {
                        new ValueComponent(string_construct),
                        new ValueComponent(colon_token),
                        new ValueComponent(valueType_group_ref),
                    }),
                    SpacerOptions.NEVER
                ),
                new ValueComponent(brace_close),
            })));
            valueType_group.Add(object_construct);

            // An non type restricted list
            var list_construct = lang.Add(new Construct("json_list", new OrderComponent(new List<IComponent>
            {
                new ValueComponent(block_open),
                new SpacedRepeatComponent(
                    new ValueComponent(comma_token),
                    new ValueComponent(valueType_group_ref),
                    SpacerOptions.NEVER
                ),
                new ValueComponent(block_close),
            })));
            valueType_group.Add(list_construct);

            lang.AddFilter(new LanguageStart(new LanguageStartArgs { KeyReference = valueType_group_ref }));

            return lang;
        }
    }
}
