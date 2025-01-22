using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    public sealed class NumberExpression : Expression{
        public NumberExpression(Token numberToken){
            _numberToken = numberToken;
        }

        public override TokenKind Kind => TokenKind.NumberExpression;
        public Token _numberToken { get; }
        public override IEnumerable<Node> GetChildren() { yield return _numberToken;}
    }
}