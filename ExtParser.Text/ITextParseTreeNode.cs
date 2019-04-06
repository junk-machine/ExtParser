using System.Collections.Generic;

namespace ExtParser.Text
{
    /// <summary>
    /// Node in the parse tree that represents a rule match.
    /// </summary>
    public interface ITextParseTreeNode
    {
        /// <summary>
        /// Gets the name of the matched rule.
        /// </summary>
        string RuleName { get; }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        ITextParseTreeNode Parent { get; }

        /// <summary>
        /// Gets the children nodes.
        /// </summary>
        IReadOnlyList<ITextParseTreeNode> Children { get; }

        /// <summary>
        /// Gets the matched text.
        /// </summary>
        /// <returns>Matched text.</returns>
        string GetImage();
    }
}
