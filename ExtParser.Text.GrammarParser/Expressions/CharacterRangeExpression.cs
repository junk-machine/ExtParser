using System;
using System.Collections.Generic;
using System.Text;

namespace ExtParser.Text.GrammarParser.Expressions
{
    internal sealed class CharacterRangeExpression : ExpressionTreeNode
    {
        public string Min { get; private set; }
        public string Max { get; private set; }

        public CharacterRangeExpression(string min, string max)
        {
            Min = min;
            Max = max;
        }

        public override void Visit(IExpressionTreeVisitor visitor)
        {
            visitor.VisitCharacterRange(this);
        }
    }
}
