using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class GrammarRule : TextProductionRule
    {
        public override string RuleName => ExtGrammarRules.Grammar;

        protected override IParserRule<char> GetBody()
        {
            return
                OneOrMoreTimes(
                    Rule(ExtGrammarRules.Rule));
        }
    }
}
