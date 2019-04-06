using ExtParser.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExtParser.Text
{
    /// <summary>
    /// Base class for all text-based parsers.
    /// </summary>
    public abstract class TextParser
    {
        /// <summary>
        /// Holds parser grammar.
        /// </summary>
        private Lazy<IGrammar<char>> grammar;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextParser"/> class.
        /// </summary>
        public TextParser()
        {
            grammar = new Lazy<IGrammar<char>>(DefineGrammar, true);
        }

        /// <summary>
        /// Parses given text input according to defined grammar.
        /// </summary>
        /// <param name="input">Text input</param>
        /// <param name="ruleName">Name of the entry-point rule</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Text parse tree if input was parsed successfully, otherwise null.</returns>
        protected async Task<ITextParseTreeNode> Parse(string input, string ruleName, CancellationToken cancellation)
        {
            var parsingContext =
                new ParsingContext<char>(
                    grammar.Value,
                    new CharacterStream(input));

            var matchingBranches =
                await grammar.Value.GetRule(ruleName).Match(parsingContext);

            if (matchingBranches == null)
            {
                return null;
            }

            if (matchingBranches.Count > 1)
            {
                throw new Exception("Ambiguous match");
            }

            return matchingBranches.Count == 1
                ? ToTextParseTree(input, matchingBranches.First().ParseTree)
                : null;
        }

        /// <summary>
        /// Converts basic parse tree to text parse tree.
        /// </summary>
        /// <param name="input">Text input</param>
        /// <param name="parseTreeNode">Parse tree for the given text input</param>
        /// <returns>Text parse tree.</returns>
        private TextParseTreeNode ToTextParseTree(string input, ParseTreeNode parseTreeNode)
        {
            var textParseTreeNode =
                new TextParseTreeNode(
                    parseTreeNode.RuleName,
                    input,
                    parseTreeNode.StartPosition,
                    parseTreeNode.EndPosition);

            foreach (var child in parseTreeNode.Children)
            {
                textParseTreeNode.AddChild(ToTextParseTree(input, child));
            }

            return textParseTreeNode;
        }

        /// <summary>
        /// Defines grammar for the parser.
        /// </summary>
        /// <returns>Parser grammar.</returns>
        protected abstract IGrammar<char> DefineGrammar();
    }
}
