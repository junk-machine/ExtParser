using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class OptionalExpressionRule : TextProductionRule
    {
        public override string RuleName => ExtGrammarRules.OptionalExpression;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Literal("["),
                    OneOrMoreTimes(Rule(ExtGrammarRules.Expression)),
                    Literal("]"));
        }
    }
}
