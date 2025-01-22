using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    public sealed class ParenExpression : Expression{
        public Token OpenParen{get;}
        public Expression Expression{get;}
        public Token CloseParen{get;}

        public ParenExpression(Token openParen, Expression expression, Token closeParen){
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }

        public override TokenKind Kind => TokenKind.ParenExpression; 
        public override IEnumerable<Node> GetChildren()
        {
            yield return OpenParen;
            yield return Expression;
            yield return CloseParen;
        }
    }

}