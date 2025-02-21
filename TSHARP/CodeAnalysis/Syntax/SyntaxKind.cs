
namespace TSharp.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        //Tokens
        EndOfFileToken,
        BadToken,
        NumberToken,
        StringToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        MultiplyToken,
        DivisionToken,
        NotToken,
        AndToken,
        OrToken,
        EqualsEqualsToken,
        NotEqualsToken,
        BiggerToken,
        SmallerToken,
        EqualOreBiggerToken,
        EqualOreSmallerToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        IdentifierToken,
        EqualsToken,

        //Keyword
        TrueKeyword,
        FalseKeyword,
        VarKeyword,
        LetKeyword,
        IfKeyword,
        ElseKeyword,
        ForKeyword,
        ToKeyword,
        WhileKeyword,

        //Nodes
        CompilationUnit,

        //Statements
        BlockStatement,
        IfStatement,
        ElseClause,
        ForStatement,
        WhileStatement,
        ExpressionStatement,
        VariableDeclaration,


        //Expressions
        LiteralExpression,
        AssignmentExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}
