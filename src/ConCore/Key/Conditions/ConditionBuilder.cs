using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ConCore.Key.Conditions
{
    public readonly struct TemplateCondition
    {
        public readonly Func<ConditionArgs, ReadCondition> Build;

        public TemplateCondition(Func<ConditionArgs, ReadCondition> build)
        {
            Build = build;
        }
    }

    public class ConditionBuilder
    {
        public TemplateCondition Empty()
        {
            return new TemplateCondition((props) => new EmptyCondition());
        }

        public TemplateCondition Regex(string pattern)
        {
            return new TemplateCondition((props) => new RegexCondition(new Regex(pattern, RegexOptions.Multiline)));
        }

        public TemplateCondition Indent(int depthChange, string[]? indentSnippets)
        {
            return new TemplateCondition((props) =>
            {
                if (indentSnippets != null)
                {
                    props.indentSnippets = indentSnippets;
                }
                props.depth += depthChange;
                return new IndentingCondition(props);
            });
        }

        public TemplateCondition Or(IReadOnlyList<TemplateCondition> conditions)
        {
            return new TemplateCondition((props) => new OrCondition(conditions.Select((c) => c.Build(props)).ToArray()));
        }

        public TemplateCondition Order(IReadOnlyList<TemplateCondition> conditions)
        {
            return new TemplateCondition((props) => new OrderCondition(conditions.Select((c) => c.Build(props)).ToArray()));
        }
    }
}
