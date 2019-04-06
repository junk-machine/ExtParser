using ExtParser.Core;
using System.Text;
using System.Threading.Tasks;

namespace ExtParser.Text.GrammarParser
{
    sealed class RuleNameRule : TerminalParserRule<char>
    {
        public override string RuleName => ExtGrammarRules.RuleName;

        protected override async Task<bool> TryMatch(IParsingContext<char> context)
        {
            var matched = false;

            while ((context.TokenStream.CurrentToken >= 'a' && context.TokenStream.CurrentToken <= 'z')
                || (context.TokenStream.CurrentToken >= 'A' && context.TokenStream.CurrentToken <= 'Z')
                || (context.TokenStream.CurrentToken >= '0' && context.TokenStream.CurrentToken <= '9')
                || context.TokenStream.CurrentToken == '_')
            {
                await context.TokenStream.Consume();
                matched = true;
            }

            return matched;
        }
    }
}
