using System;
using System.Collections.Generic;
using System.Text;

namespace ExtParser.Text.GrammarParser.Expressions
{
    internal interface IExpressionTreeVisitor
    {
        void VisitSequence(SequenceExpression expression);
        void VisitRule(RuleExpression expression);
        void VisitOptional(OptionalExpression expression);
        void VisitOneOrMoreTimes(OneOrMoreTimesExpression expression);
        void VisitZeroOrMoreTimes(ZeroOrMoreTimesExpression expression);
        void VisitOneOf(OneOfExpression expression);
        void VisitLiteral(LiteralExpression expression);
        void VisitCharacterRange(CharacterRangeExpression expression);
    }
}
