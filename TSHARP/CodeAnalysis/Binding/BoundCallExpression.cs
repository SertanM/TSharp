﻿using System.Collections.Immutable;
using TSharp.CodeAnalysis.Symbols;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        

        public override BoundNodeKind Kind => BoundNodeKind.CallExpression;
        public override TypeSymbol Type => Function.Type;
        public FunctionSymbol Function { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }
    }
}