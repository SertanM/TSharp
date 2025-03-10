namespace TSharp.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind) 
        {
            switch (kind) 
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.NotToken:
                    return 6;


                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.MultiplyToken:
                case SyntaxKind.DivisionToken:
                    return 5;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;

                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.NotEqualsToken:
                case SyntaxKind.BiggerToken:
                case SyntaxKind.SmallerToken:
                case SyntaxKind.EqualOreBiggerToken:
                case SyntaxKind.EqualOreSmallerToken:
                    return 3;

                case SyntaxKind.AndToken:
                    return 2;
                case SyntaxKind.OrToken:
                    return 1;

                default:
                    return 0;
            }
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                case "var":
                    return SyntaxKind.VarKeyword;
                case "let":
                    return SyntaxKind.LetKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "for":
                    return SyntaxKind.ForKeyword;
                case "to":
                    return SyntaxKind.ToKeyword;
                case "while":
                    return SyntaxKind.WhileKeyword;
                case "func":
                    return SyntaxKind.FunctionKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.EndOfFileToken:
                    return "\0";
                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.MultiplyToken:
                    return "*";
                case SyntaxKind.DivisionToken:
                    return "/";
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";
                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";
                case SyntaxKind.ColonToken:
                    return ":";
                case SyntaxKind.CommaToken:
                    return ",";
                case SyntaxKind.AndToken:
                    return "&&";
                case SyntaxKind.OrToken:
                    return "||";
                case SyntaxKind.NotToken:
                    return "!";
                case SyntaxKind.NotEqualsToken:
                    return "!=";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.VarKeyword:
                    return "var";
                case SyntaxKind.LetKeyword:
                    return "let";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.ForKeyword:
                    return "for";
                case SyntaxKind.ToKeyword:
                    return "to";
                case SyntaxKind.WhileKeyword:
                    return "while";
                case SyntaxKind.FunctionKeyword:
                    return "func";
                default:
                    return null;
            }
        }
    }
}
