using ExtParser.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ExtParser.Text.GrammarParser
{
    /// <summary>
    /// Parser for ExtParser grammars.
    /// </summary>
    public sealed class ExtGrammarParser : TextParser
    {
        /// <summary>
        /// Parses grammar for ExtParser grammar.
        /// </summary>
        /// <param name="input">Context-free grammar</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Text parse tree if input was parsed successfully, otherwise null.</returns>
        public async Task<ITextParseTreeNode> Parse(string input, CancellationToken cancellation)
        {
            return await Parse(input, ExtGrammarRules.Grammar, cancellation);
        }

        /// <summary>
        /// Defines grammar for ExtParser grammar.
        /// </summary>
        /// <returns>ExtParser grammar grammar.</returns>
        protected override IGrammar<char> DefineGrammar()
        {
            var grammar = new Grammar<char>();

            grammar.AddRule(new WhitespaceRule(), isGlobal: true);

            grammar.AddRule(new GrammarRule());

            grammar.AddRule(new RuleRule());
            grammar.AddRule(new RuleNameRule());
            grammar.AddRule(new IsGlobalRuleRule());
            grammar.AddRule(new ExcludedGlobalRulesRule());
            grammar.AddRule(new RuleBodyRule());

            grammar.AddRule(new ExpressionRule());
            grammar.AddRule(new ExpressionGroupRule());
            grammar.AddRule(new OptionalExpressionRule());
            grammar.AddRule(new AlternateExpressionRule());

            grammar.AddRule(new ZeroOrMoreTimesRule());
            grammar.AddRule(new OneOrMoreTimesRule());

            grammar.AddRule(new LiteralRule());
            grammar.AddRule(new LiteralValueRule());

            grammar.AddRule(new CharacterRangeRule());
            grammar.AddRule(new CharacterRule());
            grammar.AddRule(new CharacterValueRule());

            return grammar;
        }
    }
}
