using System;
using System.IO;

namespace ExtParser.Text
{
    /// <summary>
    /// Parse tree walker that prints out rule names that have matched.
    /// </summary>
    public class DebugTextParseTreeWalker : TextParseTreeWalker
    {
        /// <summary>
        /// A <see cref="TextWriter"/> to wrtie the output to.
        /// </summary>
        private readonly TextWriter output;

        /// <summary>
        /// Current output indentation level (tree depth).
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugTextParseTreeWalker"/> class
        /// with the provided output writer.
        /// </summary>
        /// <param name="output">Writer to print out rule names</param>
        public DebugTextParseTreeWalker(TextWriter output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        /// <summary>
        /// Writes the rule name to the output and enters all the matched children rules.
        /// </summary>
        /// <param name="match">Match of the text parser rule</param>
        protected sealed override void Enter(ITextParseTreeNode match)
        {
            WriteMatch(match);

            ++indentLevel;
            base.Enter(match);
            --indentLevel;
        }

        /// <summary>
        /// Writes the rule name with the current indentation level.
        /// </summary>
        /// <param name="match">Match of the text parser rule</param>
        protected virtual void WriteMatch(ITextParseTreeNode match)
        {
            Write(match.RuleName);
        }

        /// <summary>
        /// Writes given <paramref name="matchText"/> with the current indentation level.
        /// </summary>
        /// <param name="matchText">Text to write</param>
        protected void Write(string matchText)
        {
            output.WriteLine(new string('-', indentLevel) + matchText);
        }
    }
}
