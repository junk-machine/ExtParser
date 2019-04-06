using System;
using System.Collections.Generic;

namespace ExtParser.Core
{
    /// <summary>
    /// Parse tree node.
    /// </summary>
    public sealed class ParseTreeNode
    {
        /// <summary>
        /// List of children parse tree nodes.
        /// </summary>
        private List<ParseTreeNode> children;

        /// <summary>
        /// Gets the name of the matched rule.
        /// </summary>
        public string RuleName { get; private set; }

        /// <summary>
        /// Gets the start token position of the match.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// Gets the end token position of the match.
        /// </summary>
        public int EndPosition { get; set; }

        /// <summary>
        /// Gets the parent parse tree node.
        /// </summary>
        public ParseTreeNode Parent { get; private set; }

        /// <summary>
        /// Gets the list of children parse tree nodes.
        /// </summary>
        public IEnumerable<ParseTreeNode> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseTreeNode"/> class
        /// with the provided rule, start and end token positions of the match.
        /// </summary>
        /// <param name="ruleName">Name of the matched rule</param>
        /// <param name="startPosition">Start token position</param>
        /// <param name="endPosition">End token position</param>
        public ParseTreeNode(string ruleName, int startPosition, int endPosition)
        {
            RuleName = ruleName ?? throw new ArgumentNullException(nameof(ruleName));
            StartPosition = startPosition;
            EndPosition = endPosition;

            children = new List<ParseTreeNode>();
        }

        /// <summary>
        /// Adds a child node to the tree.
        /// </summary>
        /// <param name="node">Child node to add</param>
        public void AddChild(ParseTreeNode node)
        {
            node.Parent = this;
            children.Add(node);
        }

        /// <summary>
        /// Removes child node from the tree.
        /// </summary>
        /// <param name="node">Child node to remove</param>
        public void RemoveChild(ParseTreeNode node)
        {
            if (children.Remove(node))
            {
                node.Parent = null;
            }
        }

        /// <summary>
        /// Clones parse tree node and all of its children.
        /// </summary>
        /// <param name="excludedChild">Child node to exclude when cloning</param>
        /// <returns>Copy of this parse tree node with all its children.</returns>
        public ParseTreeNode Clone(ParseTreeNode excludedChild = null)
        {
            var newRoot = new ParseTreeNode(RuleName, StartPosition, EndPosition);

            foreach (var child in children)
            {
                if (child != excludedChild)
                {
                    newRoot.AddChild(child.Clone());
                }
            }

            return newRoot;
        }
    }
}
