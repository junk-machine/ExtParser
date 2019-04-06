using ExtParser.Core;
using System;
using System.Collections.Generic;

namespace ExtParser.Text.GrammarParser
{
    internal class CharacterRangeRule : TextProductionRule
    {
        protected override ISet<string> ExcludedGlobalRules => AllGlobalRules;

        public override string RuleName => ExtGrammarRules.CharacterRange;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Rule(ExtGrammarRules.Character),
                    Literal(".."),
                    Rule(ExtGrammarRules.Character));
        }
    }
}
