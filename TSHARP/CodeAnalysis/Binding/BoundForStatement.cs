using TSharp.CodeAnalysis.Symbols;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundForStatement : BoundLoopStatement
    {
        public BoundForStatement(VariableSymbol variable, BoundExpression startExpression, BoundExpression targetExpression, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel)
            : base(breakLabel, continueLabel)
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
