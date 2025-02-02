
namespace TSharp.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        //Tokens
        EndOfFileToken,
        BadToken,
        NumberToken,
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
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,
        IdentifierToken,

        //Keyword
        TrueKeyword,
        FalseKeyword,

        //Nodes
        CompilationUnit,

        //Statements
        BlockStatement,
        ExpressionStatement,


        //Expressions
        LiteralExpression,
        AssignmentExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        EqualsToken
    }
}
