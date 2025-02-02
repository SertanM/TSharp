
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
        OpenParenthesisToken,
        CloseParenthesisToken,
        IdentifierToken,



        //Keyword
        TrueKeyword,
        FalseKeyword,



        //Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}
