
namespace TSharp.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;
        private DiagnosticBag _diagnostics = new DiagnosticBag();

        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Current => Peek(0);
        
        private char Lookahead => Peek(1);

        private char Peek(int offset = 0)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }

        private void Next()
            => _position++;


        public SyntaxToken Lex()
        {
            var start = _position;

            if (char.IsDigit(Current))
            {

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;

                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if (char.IsLetter(Current))
            {

                while (char.IsLetter(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.MultiplyToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.DivisionToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                case '&':
                    if (Lookahead != '&') break;
                    _position++;
                    return new SyntaxToken(SyntaxKind.AndToken, _position++, "&&", null);
                case '|':
                    if(Lookahead != '|') break;
                    _position+=2;
                    return new SyntaxToken(SyntaxKind.OrToken, start, "||", null);
                case '!':
                    if (Lookahead != '=') return new SyntaxToken(SyntaxKind.NotToken, _position++, "!", null);
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.NotEqualsToken, start, "!=", null);
                case '=':
                    if (Lookahead != '=') break;
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                case '\0':
                    return new SyntaxToken(SyntaxKind.EndOfFileToken, _position++, "\0", null);
            }

            _diagnostics.ReportBadToken(_position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}
