using System.Collections.Immutable;
using TSharp.CodeAnalysis.Symbols;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope previous, ImmutableArray<Diagnostic> diagnostics, ImmutableArray<VariableSymbol> variables, ImmutableArray<FunctionSymbol> functions, BoundStatement statement)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            Variables = variables;
            Functions = functions;
            Statement = statement;
        }

        public BoundGlobalScope Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public ImmutableArray<FunctionSymbol> Functions { get; }
        public BoundStatement Statement { get; }
    }
}
