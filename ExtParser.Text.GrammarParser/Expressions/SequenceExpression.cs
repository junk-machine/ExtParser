namespace ExtParser.Text.GrammarParser.Expressions
{
    internal sealed class SequenceExpression : ExpressionTreeNode
    {
        public override void Visit(IExpressionTreeVisitor visitor)
        {
            visitor.VisitSequence(this);
        }
    }
}
