using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    public sealed class Evalutor{
        private readonly Expression _root;
        public Evalutor(Expression root)
        {
            _root = root;
        }

        public int Evalute(){
            return EvaluteExpression(_root);
        }

        private int EvaluteExpression(Expression root){
            if(root is NumberExpression n){
                return (int) n._numberToken.Value;
            }
            if(root is UnaryExpression u){
                var operand = EvaluteExpression(u.Operand);

                if(u.OpToken.Kind == TokenKind.Minus)
                    return -operand;
                
                else if(u.OpToken.Kind == TokenKind.Plus)
                    return operand;
                else
                    throw new InvalidOperationException($"Beklenmeyen unary (tekli) operator: {u.OpToken.Kind}");
                
            }

            if(root is BinaryExpression b){
                var left = EvaluteExpression(b._left);
                var right = EvaluteExpression(b._right);

                
                if(b._opToken.Kind==TokenKind.Plus){
                    return left + right;
                }
                if(b._opToken.Kind==TokenKind.Minus){
                    return left - right;
                }
                if(b._opToken.Kind==TokenKind.Multi){
                    return left * right;
                }
                if(b._opToken.Kind==TokenKind.Div){
                    return left / right;
                }
                else{
                    throw new Exception($"Beklenmeyen binary (ikili) operatör, {b._opToken.Kind}");
                } 
            }

            if(root is ParenExpression p){
                return EvaluteExpression(p.Expression);
            }
            throw new Exception($"Beklenmeyen node, {root.Kind}");
        }
    }
}