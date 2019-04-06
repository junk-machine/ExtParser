using ExtParser.Core;
using System.Threading.Tasks;

namespace ExtParser.Text.GrammarParser
{
    internal sealed class CharacterValueRule : TerminalParserRule<char>
    {
        public override string RuleName => ExtGrammarRules.CharacterValue;

        protected override async Task<bool> TryMatch(IParsingContext<char> context)
        {
            if ((context.TokenStream.CurrentToken >= '\u0000' && context.TokenStream.CurrentToken <= '\u0026')
                || (context.TokenStream.CurrentToken >= '\u0027' && context.TokenStream.CurrentToken <= '\u005b')
                || (context.TokenStream.CurrentToken >= '\u005d' && context.TokenStream.CurrentToken <= '\uFFFF'))
            {
                await context.TokenStream.Consume();
                return true;
            }
            else if (context.TokenStream.CurrentToken == '\\')
            {
                await context.TokenStream.Consume();

                if (context.TokenStream.CurrentToken == '\\'
                    || context.TokenStream.CurrentToken == '\'')
                {
                    await context.TokenStream.Consume();
                    return true;
                }
                else if (context.TokenStream.CurrentToken == 'u')
                {
                    await context.TokenStream.Consume();

                    for (var hexIndex = 0; hexIndex < 4; ++hexIndex)
                    {
                        if (context.TokenStream.CurrentToken < '0' && context.TokenStream.CurrentToken > '9'
                            && context.TokenStream.CurrentToken < 'A' && context.TokenStream.CurrentToken > 'F')
                        {
                            return false;
                        }

                        await context.TokenStream.Consume();
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
