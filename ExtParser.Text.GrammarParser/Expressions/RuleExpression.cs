using System;
using System.Collections.Generic;
using System.Text;

namespace ExtParser.Text.GrammarParser.Expressions
{
    internal sealed class RuleExpression : ExpressionTreeNode
    {
        public string RuleName { get; private set; }

        public RuleExpression(string ruleName)
        {
            RuleName = ruleName ?? throw new ArgumentNullException(nameof(ruleName));
        }

        public override void Visit(IExpressionTreeVisitor visitor)
        {
            visitor.VisitRule(this);
        }
    }
}
