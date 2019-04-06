using ExtParser.Core;
using System;
using System.Collections.Generic;

namespace ExtParser.Text.GrammarParser
{
    internal class CharacterRule : TextProductionRule
    {
        protected override bool IsAtomicRule => true;

        protected override ISet<string> ExcludedGlobalRules => AllGlobalRules;

        public override string RuleName => ExtGrammarRules.Character;

        protected override IParserRule<char> GetBody()
        {
            return
                Sequence(
                    Literal("'"),
                    Rule(ExtGrammarRules.CharacterValue),
                    Literal("'"));
        }
    }
}
