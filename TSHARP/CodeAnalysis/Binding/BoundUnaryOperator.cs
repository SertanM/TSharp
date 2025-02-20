﻿using TSharp.CodeAnalysis.Syntax;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, System.Type operandType) 
            : this(syntaxKind, kind, operandType, operandType){}

        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, System.Type operandType, System.Type resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandType;
            Type = resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public System.Type OperandType { get; }
        public System.Type Type { get; }

        private static BoundUnaryOperator[] _operators = 
        {
            new BoundUnaryOperator(SyntaxKind.NotToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int))
        };

        public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, System.Type operandType) 
        {
            foreach (var op in _operators) 
                if(op.SyntaxKind == syntaxKind && op.OperandType == operandType)
                    return op;
            
            return null;
        }
    }
}
