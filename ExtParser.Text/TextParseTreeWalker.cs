namespace ExtParser.Text
{
    /// <summary>
    /// Base class for walkers of the text parse trees.
    /// </summary>
    public abstract class TextParseTreeWalker
    {
        /// <summary>
        /// Walks the parse tree over the given input.
        /// </summary>
        /// <param name="parseTree">Parse tree for the given input</param>
        public void Walk(ITextParseTreeNode parseTree)
        {
            Enter(parseTree);
        }

        /// <summary>
        /// Enters the parse tree node, that represents matched rule.
        /// </summary>
        /// <param name="match">Match of the text parser rule</param>
        protected virtual void Enter(ITextParseTreeNode match)
        {
            foreach (var child in match.Children)
            {
                Enter(child);
            }
        }
    }
}
