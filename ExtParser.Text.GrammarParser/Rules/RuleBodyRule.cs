using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class RuleBodyRule : TextProductionRule
    {
        public override string RuleName => ExtGrammarRules.RuleBody;

        protected override IParserRule<char> GetBody()
        {
            return
                OneOrMoreTimes(
                    Rule(ExtGrammarRules.Expression));
        }
    }
}
