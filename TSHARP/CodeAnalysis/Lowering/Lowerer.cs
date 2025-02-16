using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Binding;
using System.Collections.Immutable;


namespace TSharp.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private Lowerer() {}

        public static BoundStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.StartExpression);
            var variableExpression = new BoundVariableExpression(node.Variable);
            
            var condition = new BoundBinaryExpression
                (
                    variableExpression,
                    BoundBinaryOperator.Bind(SyntaxKind.EqualOreSmallerToken, typeof(int), typeof(int)),
                    node.TargetExpression
                );

            var increment = new BoundExpressionStatement
                (
                    new BoundAssignmentExpression
                    (
                        node.Variable,
                        new BoundBinaryExpression
                        (
                            variableExpression,
                            BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int)),
                            new BoundLiteralExpression(1)
                        )
                    )
                );

            var whileBody = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, increment));
            var whileStatement = new BoundWhileStatement(condition, whileBody);
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(variableDeclaration, whileStatement));
            return RewriteStatement(result);
        }
    }
}