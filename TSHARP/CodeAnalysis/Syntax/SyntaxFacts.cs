


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
                default:
                    return null;
            }
        }
    }
}
