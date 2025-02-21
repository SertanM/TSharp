
using System.Text;
using TSharp.CodeAnalysis.Symbols;
using TSharp.CodeAnalysis.Text;

namespace TSharp.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly SourceText _text;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;

        private int _start;
        private SyntaxKind _kind;
        private object _value;

        public Lexer(SourceText text)
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
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;
            string text = null;

            if (char.IsLetter(Current))
            {
                ReadIdentifierOrKeyword();
            }
            else
            {
                switch (Current)
                {
                    case '\0':
                        _kind = SyntaxKind.EndOfFileToken;
                        break;
                    case '+':
                        _kind = SyntaxKind.PlusToken;
                        break;
                    case '-':
                        _kind = SyntaxKind.MinusToken;
                        break;
                    case '*':
                        _kind = SyntaxKind.MultiplyToken;
                        break;
                    case '/':
                        _kind = SyntaxKind.DivisionToken;
                        break;
                    case '(':
                        _kind = SyntaxKind.OpenParenthesisToken;
                        break;
                    case ')':
                        _kind = SyntaxKind.CloseParenthesisToken;
                        break;
                    case '{':
                        _kind = SyntaxKind.OpenBraceToken;
                        break;
                    case '}':
                        _kind = SyntaxKind.CloseBraceToken;
                        break;
                    case '&':
                        if (Lookahead != '&') break;
                        _position++;
                        _kind = SyntaxKind.AndToken;
                        break;
                    case '|':
                        if (Lookahead != '|') break;
                        _position++;
                        _kind = SyntaxKind.OrToken;
                        break;
                    case '!':
                        if (Lookahead != '=') 
                        {
                            _kind = SyntaxKind.NotToken; 
                            break;
                        }
                        _position++;
                        _kind = SyntaxKind.NotEqualsToken;
                        break;
                    case '=':
                        if (Lookahead != '=')
                        {
                            _kind = SyntaxKind.EqualsToken;
                            break;
                        }
                        _position++;
                        _kind = SyntaxKind.EqualsEqualsToken;
                        break;
                    case '>':
                        if(Lookahead != '=')
                        {
                            _kind = SyntaxKind.BiggerToken;
                            break;
                        }
                        _position++;
                        _kind = SyntaxKind.EqualOreBiggerToken;
                        break;
                    case '<':
                        if (Lookahead != '=')
                        {
                            _kind = SyntaxKind.SmallerToken;
                            break;
                        }
                        _position++;
                        _kind = SyntaxKind.EqualOreSmallerToken;
                        break;
                    case '"':
                        ReadStringTokens();
                        _position--;
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        ReadNumberTokens();
                        _position--;
                        break;
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        ReadWhiteSpaces();
                        _position--;
                        break;
                    default:
                        _diagnostics.ReportBadToken(_position, Current);
                        break;
                }
                _position++;
            }

            var length = _position - _start;
            text = SyntaxFacts.GetText(_kind);
            if(text== null) 
                text = _text.ToString(_start, length);

            return new SyntaxToken(_kind, _start, text, _value);
        }

        

        private void ReadWhiteSpaces()
        {
            while (char.IsWhiteSpace(Current))
                Next();

            _kind = SyntaxKind.WhiteSpaceToken;
        }

        private void ReadStringTokens()
        {
            _position++;
            var sb = new StringBuilder();

            var isDone = false;
            while (!isDone)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(_start, 1);
                        _diagnostics.ReportUnterminatedString(span);
                        isDone = true;
                        break;
                    case '"':
                        isDone = true; 
                        _position++;
                        break;
                    default:
                        sb.Append(Current);
                        _position++;
                        break;
                }

            }

            _kind = SyntaxKind.StringToken;
            _value = sb.ToString();
        }

        private void ReadNumberTokens()
        {
            while (char.IsDigit(Current))
                Next();

            var length = _position - _start;

            var text = _text.ToString(_start, length);
            if (!int.TryParse(text, out var value))
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, TypeSymbol.Int);
            
            _value = value;
            _kind = SyntaxKind.NumberToken;
        }
        
        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
                Next();

            var text = _text.ToString(_start, _position - _start);

            _kind = SyntaxFacts.GetKeywordKind(text);
        }
    }
}
