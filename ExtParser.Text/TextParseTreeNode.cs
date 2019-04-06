using System;
using System.Collections.Generic;

namespace ExtParser.Text
{
    /// <summary>
    /// Parse tree node for the parsed text input.
    /// </summary>
    internal sealed class TextParseTreeNode : ITextParseTreeNode
    {
        private readonly string input;
        private readonly int startPosition;
        private readonly int endPosition;

        private readonly List<ITextParseTreeNode> children;

        /// <summary>
        /// Gets the name of the matched rule.
        /// </summary>
        public string RuleName { get; private set; }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public ITextParseTreeNode Parent { get; private set; }

        /// <summary>
        /// Gets the children nodes.
        /// </summary>
        public IReadOnlyList<ITextParseTreeNode> Children => children;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextParseTreeNode"/> class
        /// with the provided rule type, input text, start and end character positions.
        /// </summary>
        /// <param name="ruleName">Name of the matched rule</param>
        /// <param name="input">Text input</param>
        /// <param name="startPosition">Start character position</param>
        /// <param name="endPosition">End character position</param>
        public TextParseTreeNode(string ruleName, string input, int startPosition, int endPosition)
        {
            RuleName =
                ruleName ?? throw new ArgumentNullException(nameof(ruleName));

            this.input =
                input ?? throw new ArgumentNullException(nameof(input));

            this.startPosition = startPosition;
            this.endPosition = endPosition;

            children = new List<ITextParseTreeNode>();
        }

        /// <summary>
        /// Adds child parse tree node to the tree.
        /// </summary>
        /// <param name="node">Node to add</param>
        public void AddChild(TextParseTreeNode node)
        {
            node.Parent = this;
            children.Add(node);
        }

        /// <summary>
        /// Gets the matched text.
        /// </summary>
        /// <returns>Matched text.</returns>
        public string GetImage()
        {
            return input.Substring(startPosition, endPosition - startPosition);
        }
    }
}
