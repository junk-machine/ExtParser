using System;
using System.Collections.Generic;
using System.Text;

namespace ExtParser.Text.GrammarParser.Expressions
{
    internal sealed class ZeroOrMoreTimesExpression : ExpressionTreeNode
    {
        public override void Visit(IExpressionTreeVisitor visitor)
        {
            visitor.VisitZeroOrMoreTimes(this);
        }
    }
}
