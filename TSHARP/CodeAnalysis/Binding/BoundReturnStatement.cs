namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundReturnStatement : BoundStatement
    {
        public BoundReturnStatement(BoundExpression expression)
        {
            Expression = expression;
        }

        public BoundExpression Expression { get; }
        public override BoundNodeKind Kind => BoundNodeKind.ReturnStatement;
    }
}
