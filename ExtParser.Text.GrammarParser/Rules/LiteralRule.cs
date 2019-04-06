using ExtParser.Core;
using System.Collections.Generic;

namespace ExtParser.Text.GrammarParser
{
    internal class LiteralRule : TextProductionRule
    {
        protected override bool IsAtomicRule => true;

        protected override ISet<string> ExcludedGlobalRules => AllGlobalRules;

        public override string RuleName => ExtGrammarRules.Literal;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Literal("\""),
                    Rule(ExtGrammarRules.LiteralValue),
                    Literal("\""));
        }
    }
}
