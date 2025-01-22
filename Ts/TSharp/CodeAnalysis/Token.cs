using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSharp.CodeAnalysis
{
    public sealed class Token : Node{
        public Token(TokenKind kind, int pos, string text, object value){
            this.Kind = kind;
            this.Pos = pos;
            this.Text = text;
            this.Value = value;
        }

        public override string ToString() {
            if(Value!=null){
                return string.Format("{0}:{1} ", Kind, Value);
            }
            return string.Format("{0} ", Kind);
        }

        public override TokenKind Kind { get; }
        public int Pos { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<Node> GetChildren(){
            return Enumerable.Empty<Node>();
        }
    }
}