namespace TSharp.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //Statements
        BlockStatement,
        IfStatement,
        ForStatement,
        WhileStatement,
        ExpressionStatement,
        VariableDeclaration,


        //Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        AssignmentExpression,
        VariableExpression
    }
}
