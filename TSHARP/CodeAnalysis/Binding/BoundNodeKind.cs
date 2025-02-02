namespace TSharp.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //Statements
        BlockStatement,
        ExpressionStatement,

        //Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        AssignmentExpression,
        VariableExpression
    }
}
