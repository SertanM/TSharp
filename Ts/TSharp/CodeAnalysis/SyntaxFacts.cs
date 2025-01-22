using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSharp.CodeAnalysis
{
    internal static class SyntaxFacts{
        //-1 * 3
        public static int GetUnaryOperatorPredence(this TokenKind kind){
            switch(kind){
                case TokenKind.Plus:
                case TokenKind.Minus:
                    return 3;
                default:
                    return 0;
            }
        }
        
        public static int GetBinaryOperatorPredence(this TokenKind kind){
            switch(kind){
                case TokenKind.Multi:
                case TokenKind.Div:
                    return 2;
                case TokenKind.Plus:
                case TokenKind.Minus:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}