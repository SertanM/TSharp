
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

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is LiteralExpressionSyntax n)
                return (int)n.LiteralToken.Value;

            if (node is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            if(node is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);

                if (u.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return operand;

                if (u.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return -operand;

                throw new Exception($"Unexcepted unary operator {u.OperatorToken.Kind}!");
            }

            if (node is BinaryExpressionSyntax b)
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

                throw new Exception($"Unexcepted binary operator {b.OperatorToken.Kind}!");
            }

            throw new Exception($"Unexcepted expression {node.Kind}");
        }
    }
}
