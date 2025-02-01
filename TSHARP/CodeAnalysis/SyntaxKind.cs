
namespace TSharp.CodeAnalysis
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
        OpenParenthesisToken,
        CloseParenthesisToken,


        
        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}
