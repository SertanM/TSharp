namespace TSharp.CodeAnalysis
{
    public enum TokenKind{
        
        Int,

        Plus,
        Minus,
        Multi,
        Div,
        LeftParan,
        RightParan,


        BadToken,
        EOF, 


        NumberExpression,
        BinaryExpression,
        UnaryExpression,
        ParenExpression

    }
}