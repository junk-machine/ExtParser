using System.IO;

namespace ExtParser.Text.GrammarParser
{
    /// <summary>
    /// Parse tree walker that prints out rule name with some additional information
    /// for the ExtParser grammar parse tree.
    /// </summary>
    public sealed class ExtGrammarDebugWalker : DebugTextParseTreeWalker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtGrammarDebugWalker"/> class
        /// with the provided output writer.
        /// </summary>
        /// <param name="output">Writer to print out rule names with additional information</param>
        public ExtGrammarDebugWalker(TextWriter output)
            : base(output)
        {
        }

        /// <summary>
        /// Writes rule name with additional information (like string literal or character range)
        /// to the output and enters all the matched children rules.
        /// </summary>
        /// <param name="match">Match of the text parser rule</param>
        protected override void WriteMatch(ITextParseTreeNode match)
        {
            if (match.RuleName == ExtGrammarRules.RuleName)
            {
                Write(match.RuleName + " (" + match.GetImage() + ")");
            }
            else if (match.RuleName == ExtGrammarRules.LiteralValue
                || match.RuleName == ExtGrammarRules.CharacterRange)
            {
                Write(match.RuleName + " " + match.GetImage());
            }
            else
            {
                base.WriteMatch(match);
            }
        }
    }
}
