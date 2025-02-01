
namespace TSharp.CodeAnalysis
{
    public sealed class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax root)
        {
            if (root is LiteralExpressionSyntax n)
                return (int)n.LiteralToken.Value;

            if (root is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            if (root is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                if (b.OperatorToken.Kind == SyntaxKind.MultiplyToken)
                    return left * right;
                if (b.OperatorToken.Kind == SyntaxKind.DivisionToken)
                    return left / right;

                throw new Exception($"Unexcepted operator {b.OperatorToken.Kind}!");
            }

            throw new Exception($"Unexcepted expression {root.Kind}");
        }
    }
}
