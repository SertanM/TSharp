
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
        ColonToken,
        CommaToken,
        IdentifierToken,
        EqualsToken,

        //Keyword
        TrueKeyword,
        FalseKeyword,
        VarKeyword,
        LetKeyword,
        ContinueKeyword,
        BreakKeyword,
        IfKeyword,
        ElseKeyword,
        ForKeyword,
        ToKeyword,
        WhileKeyword,
        FunctionKeyword,
        ReturnKeyword,

        //Nodes
        CompilationUnit,
        Parameter,
        ElseClause,
        TypeClause,

        //Statements
        BlockStatement,
        IfStatement,
        ForStatement,
        WhileStatement,
        ExpressionStatement,
        VariableDeclaration,
        FunctionDeclaration,
        GlobalStatement,
        BreakStatement,
        ContinueStatement,
        ReturnStatement,

        //Expressions
        LiteralExpression,
        AssignmentExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        CallExpression
    }
}
