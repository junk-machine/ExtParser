using ExtParser.Core;
using System.Threading.Tasks;

namespace ExtParser.Text.GrammarParser
{
    sealed class WhitespaceRule : TerminalParserRule<char>
    {
        protected override bool IsAtomicRule => true;

        public override string RuleName => ExtGrammarRules.Whitespace;

        protected override async Task<bool> TryMatch(IParsingContext<char> context)
        {
            var matched = false;

            while (context.TokenStream.CurrentToken == ' '
                || context.TokenStream.CurrentToken == '\t'
                || context.TokenStream.CurrentToken == '\n'
                || context.TokenStream.CurrentToken == '\r')
            {
                matched = true;
                await context.TokenStream.Consume();
            }

            return matched;
        }
    }
}
