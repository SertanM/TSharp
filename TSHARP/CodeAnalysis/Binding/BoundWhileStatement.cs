namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement statement)
        {
            Condition = condition;
            Statement = statement;
        }

        public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;

        public BoundExpression Condition { get; }
        public BoundStatement Statement { get; }
    }

    internal sealed class BoundForStatement : BoundStatement
    {
        private BoundExpression startBound;
        private BoundExpression targetBound;
        private BoundNode body;

        public BoundForStatement(VariableSymbol variable, BoundExpression startExpression, BoundExpression targetExpression, BoundStatement body)
        {
            Variable = variable;
            StartExpression = startExpression;
            TargetExpression = targetExpression;
            Body = body;
            // +: Think T think about what will you have 500 years from now?
            // -: Your mom
        }


        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;

        public VariableSymbol Variable { get; }
        public BoundExpression StartExpression { get; }
        public BoundExpression TargetExpression { get; }
        public BoundStatement Body { get; }
    }
}
