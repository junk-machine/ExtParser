using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class RuleRule : TextProductionRule
    {
        public override string RuleName => ExtGrammarRules.Rule;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Rule(ExtGrammarRules.RuleName),
                    OneOf(
                        Optional(Rule(ExtGrammarRules.IsGlobalRule)),
                        Sequence(
                            Optional(
                                OneOf(
                                    Rule(ExtGrammarRules.IsGlobalRule),
                                    Rule(ExtGrammarRules.ExcludedGlobalRules))),
                            Sequence(
                                Literal("="),
                                Rule(ExtGrammarRules.RuleBody)))),
                    Literal(";"));

            /*
            return
                Sequence(
                    Rule(ExtGrammarRules.RuleName),
                    Optional(
                        OneOf(
                            Rule(ExtGrammarRules.IsGlobalRule),
                            Rule(ExtGrammarRules.ExcludedGlobalRules))),
                    Optional(
                        Sequence(
                            Literal("="),
                            Rule(ExtGrammarRules.RuleBody))),
                    Literal(";"));
            */
        }
    }
}
