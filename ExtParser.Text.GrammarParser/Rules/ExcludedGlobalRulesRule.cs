using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class ExcludedGlobalRulesRule : TextProductionRule
    {
        public override string RuleName => ExtGrammarRules.ExcludedGlobalRules;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Literal("!"),
                    Optional(
                        Sequence(
                            Literal("("),
                            Rule(ExtGrammarRules.RuleName),
                            ZeroOrMoreTimes(
                                Sequence(Literal(","), Rule(ExtGrammarRules.RuleName))),
                            Literal(")"))));
        }
    }
}
