using ExtParser.Core;
using System.Threading.Tasks;

namespace ExtParser.Text.GrammarParser
{
    internal sealed class OneOrMoreTimesRule : TerminalParserRule<char>
    {
        public override string RuleName => ExtGrammarRules.OneOrMoreTimes;

        protected override async Task<bool> TryMatch(IParsingContext<char> context)
        {
            if (context.TokenStream.CurrentToken == '+')
            {
                await context.TokenStream.Consume();
                return true;
            }

            return false;
        }
    }
}
