using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal class ExpressionRule : TextProductionRule
    {
        protected override bool IsAtomicRule => true;

        public override string RuleName => ExtGrammarRules.Expression;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    OneOf(
                        Sequence(
                            OneOf(
                                Rule(ExtGrammarRules.ExpressionGroup),
                                Rule(ExtGrammarRules.RuleName),
                                Rule(ExtGrammarRules.Literal),
                                Rule(ExtGrammarRules.CharacterRange)),
                            Optional(
                                OneOf(
                                    Rule(ExtGrammarRules.ZeroOrMoreTimes),
                                    Rule(ExtGrammarRules.OneOrMoreTimes)))),
                        Rule(ExtGrammarRules.OptionalExpression)),
                    Optional(
                        Rule(ExtGrammarRules.AlternateExpression)));
        }
    }
}
