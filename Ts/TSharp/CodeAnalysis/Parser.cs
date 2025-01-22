using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    

    internal sealed class Parser{
        private readonly Token[] _tokens;
        private int _position;
        private List<string> _diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => _diagnostics;

        public Parser(string text) {
            var tokens = new List<Token>();
            var Lexer = new Lexer(text);
            Token token;
            do {
                token = Lexer.NextToken();
                if(token.Kind != TokenKind.BadToken){
                    tokens.Add(token);
                }
            }while(token.Kind!=TokenKind.EOF);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(Lexer.Diagnotics);
        }

        private Token Peek(int offset){
            var index = _position + offset;
            if(index >= _tokens.Length) 
                return _tokens[_tokens.Length-1];
            
            return _tokens[index];
        }

        private Token Current => Peek(0);

        private Token NextToken(){
            Token current = Current;
            _position++;
            return current;
        }

        private Token Match(TokenKind kind){ 
            if(Current.Kind==kind){
                return NextToken();
            }
            
            _diagnostics.Add($"HATA: Beklenmeyen veri türü: {Current.Kind}, beklenen veri türü: '{kind}'");

            return new Token(TokenKind.BadToken, _position, null, null);
        }

        public SyntaxTree Parse(){
            var expression = ParseExpression();
            var eofToken = Match(TokenKind.EOF);
            return new SyntaxTree(_diagnostics, expression, eofToken);
        }

        private Expression ParseExpression(int parentPredence = 0){
            Expression left;
            
            var unaryOperatorPredence = Current.Kind.GetUnaryOperatorPredence();
            if(unaryOperatorPredence!=0 && unaryOperatorPredence >= parentPredence) {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryOperatorPredence);
                left = new UnaryExpression(operatorToken, operand);
            }else{
                left = ParsePrimaryExpression();
            }


            while(true){
                var predence = Current.Kind.GetBinaryOperatorPredence();
                if(predence == 0 || predence <= parentPredence) break;
                var opToken = NextToken();
                var right = ParseExpression(predence);
                left = new BinaryExpression(left, opToken, right);
            }
            
            return left;
        }

        private Expression ParsePrimaryExpression()
        {
            if(Current.Kind==TokenKind.LeftParan){
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(TokenKind.RightParan);

                return new ParenExpression(left,expression,right);
            }

            var numberToken = Match(TokenKind.Int); 
            return new NumberExpression(numberToken);
        }
    }
}