using System.Threading.Tasks;
using ExtParser.Core;

namespace ExtParser.Text.GrammarParser
{
    internal sealed class IsGlobalRuleRule : TerminalParserRule<char>
    {
        public override string RuleName => ExtGrammarRules.IsGlobalRule;

        protected override async Task<bool> TryMatch(IParsingContext<char> context)
        {
            if (context.TokenStream.CurrentToken == '*')
            {
                await context.TokenStream.Consume();
                return true;
            }

            return false;
        }
    }
}
