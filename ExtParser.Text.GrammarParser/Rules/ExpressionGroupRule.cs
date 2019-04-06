using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class ExpressionGroupRule : TextProductionRule
    {
        public override string RuleName => ExtGrammarRules.ExpressionGroup;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Literal("("),
                    OneOrMoreTimes(Rule(ExtGrammarRules.Expression)),
                    Literal(")"));
        }
    }
}
