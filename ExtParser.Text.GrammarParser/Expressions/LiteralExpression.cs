using System;
using System.Collections.Generic;
using System.Text;

namespace ExtParser.Text.GrammarParser.Expressions
{
    internal sealed class LiteralExpression : ExpressionTreeNode
    {
        public string Value { get; private set; }

        public LiteralExpression(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override void Visit(IExpressionTreeVisitor visitor)
        {
            visitor.VisitLiteral(this);
        }
    }
}
