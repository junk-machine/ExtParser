using System.Collections.Generic;

namespace ExtParser.Text.GrammarParser.Expressions
{
    internal abstract class ExpressionTreeNode
    {
        private readonly List<ExpressionTreeNode> children;

        public ExpressionTreeNode Parent { get; private set; }

        public IReadOnlyList<ExpressionTreeNode> Children
        {
            get { return children; }
        }

        public ExpressionTreeNode()
        {
            children = new List<ExpressionTreeNode>();
        }

        public void AddChild(ExpressionTreeNode node)
        {
            children.Add(node);
            node.Parent = this;
        }

        public ExpressionTreeNode RemoveLastChild()
        {
            var node = children[children.Count - 1];

            children.RemoveAt(children.Count - 1);
            node.Parent = null;

            return node;
        }

        public abstract void Visit(IExpressionTreeVisitor visitor);
    }
}
