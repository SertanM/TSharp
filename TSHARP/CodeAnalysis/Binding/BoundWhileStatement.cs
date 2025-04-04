namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundWhileStatement : BoundLoopStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement statement, BoundLabel breakLabel, BoundLabel continueLabel) 
            : base(breakLabel, continueLabel)
        {
            Condition = condition;
            Statement = statement;
        }

        public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;

        public BoundExpression Condition { get; }
        public BoundStatement Statement { get; }
    }
}