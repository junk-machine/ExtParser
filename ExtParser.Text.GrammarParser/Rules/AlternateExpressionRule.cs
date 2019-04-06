using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class AlternateExpressionRule : TextProductionRule
    {
        public override string RuleName => ExtGrammarRules.AlternateExpression;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Literal("|"),
                    Rule(ExtGrammarRules.Expression));
        }
    }
}
