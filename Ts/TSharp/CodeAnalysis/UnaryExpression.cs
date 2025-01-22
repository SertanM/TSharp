using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    public sealed class UnaryExpression : Expression{
        public UnaryExpression(Token opToken, Expression operand){
            OpToken = opToken;
            Operand = operand;
        }

        public override TokenKind Kind => TokenKind.UnaryExpression;
        public Token OpToken { get; }
        public Expression Operand { get; }
        public override IEnumerable<Node> GetChildren() {  yield return OpToken; yield return Operand; } 
        
    }
}