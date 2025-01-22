using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    public sealed class BinaryExpression : Expression{
        public BinaryExpression(Expression left, Token opToken, Expression right){
            _left = left;
            _opToken = opToken;
            _right = right;
        }

        public override TokenKind Kind => TokenKind.BinaryExpression;
        public Expression _left { get; }
        public Token _opToken { get; }
        public Expression _right { get; }
        public override IEnumerable<Node> GetChildren() { yield return _left; yield return _opToken; yield return _right; } 
        
    }

    
}