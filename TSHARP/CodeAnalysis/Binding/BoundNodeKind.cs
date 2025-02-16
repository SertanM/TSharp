namespace TSharp.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //Statements
        BlockStatement,
        VariableDeclaration,
        IfStatement,
        ForStatement,
        WhileStatement,
        ExpressionStatement,


        //Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        AssignmentExpression,
        VariableExpression
    }
}
