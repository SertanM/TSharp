using System;
using System.Globalization;

namespace TSharp.CodeAnalysis
{
    internal sealed class Lexer{
        private readonly string _text;
        private int _pos;

        private List<string> _diagnotics = new List<string>();

        public IEnumerable<string> Diagnotics => _diagnotics;
        public Lexer(string text){
            _text = text;
        }

        private char currentchar{
            get{
                if(_pos>=_text.Length){
                    return '\0';
                }
                return _text[_pos];
            }
        }

        private void Next(){
            _pos++;
        }

        public Token NextToken(){
            if(char.IsWhiteSpace(currentchar)){
                while(char.IsWhiteSpace(currentchar)) Next();
            }
            if(_pos >= _text.Length){
                return new Token(TokenKind.EOF ,_pos, "/0", null) ;
            }
            if(char.IsDigit(_text[_pos])){
                var start = _pos;
                TokenKind myKind = TokenKind.Int;
                while(char.IsDigit(currentchar)){
                    Next();
                }

                var Length = _pos - start;
                var text = _text.Substring(start, Length);
                
                if(myKind==TokenKind.Int){
                    if(!int.TryParse(text, out int value)){
                        _diagnotics.Add($"Numara {_text} Int32'de çözümlenemedi.");
                    }
                    return new Token(myKind, start, text, value);
                }else{
                    var value = float.Parse(text, CultureInfo.InvariantCulture.NumberFormat);
                    return new Token(myKind, start, text, value);
                }
                
                
            }
            
            if(currentchar=='+') {
                Next();
                return new Token(TokenKind.Plus, _pos, "+", null);
            }
            if(currentchar=='-') { 
                Next();
                return new Token(TokenKind.Minus, _pos, "-", null); 
            }
            if(currentchar=='*'){ 
                Next();
                return new Token(TokenKind.Multi, _pos, "*", null);
            }
            if(currentchar=='/'){ 
                Next();
                return new Token(TokenKind.Div, _pos, "/", null);
            }
            if(currentchar=='('){ 
                Next();
                return new Token(TokenKind.LeftParan, _pos, "(", null);
            }
            if(currentchar==')'){ 
                Next();
                return new Token(TokenKind.RightParan, _pos, ")", null);
            }
            _diagnotics.Add($"İllegal karakter hatası: {_pos}.pozisyonda {currentchar}");
            return new Token(TokenKind.BadToken, _pos++, _text.Substring(_pos-1, 1), null);
        }
    }
}