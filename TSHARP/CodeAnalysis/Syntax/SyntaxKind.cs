
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
        VarKeyword,
        LetKeyword,

        //Nodes
        CompilationUnit,

        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,


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
