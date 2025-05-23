﻿using System.Collections.Immutable;
using TSharp.CodeAnalysis.Symbols;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> _variables;
        private Dictionary<string, FunctionSymbol> _functions;

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public BoundScope Parent { get; }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if (_variables == null)
                _variables = new Dictionary<string, VariableSymbol>();

            if (String.IsNullOrEmpty(variable.Name))
                return false;

            if (_variables.ContainsKey(variable.Name))
                return false;

            _variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
            variable = null;

            if (_variables != null && _variables.TryGetValue(name, out variable))
                return true;


            if (Parent == null)
                return false;

            return Parent.TryLookupVariable(name, out variable);
        }

        public bool TryDeclareFunction(FunctionSymbol variable)
        {
            if (_functions == null)
                _functions = new Dictionary<string, FunctionSymbol>();

            if (String.IsNullOrEmpty(variable.Name))
                return false;

            if (_functions.ContainsKey(variable.Name))
                return false;

            _functions.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookupFunction(string name, out FunctionSymbol function)
        {
            function = null;

            if (_functions != null && _functions.TryGetValue(name, out function))
                return true;

            if (Parent == null)
                return false;

            return Parent.TryLookupFunction(name, out function);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            if(_variables == null)
                return ImmutableArray<VariableSymbol>.Empty;
            

            return _variables.Values.ToImmutableArray(); 
        }


        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            if (_functions == null)
                return ImmutableArray<FunctionSymbol>.Empty;

            return _functions.Values.ToImmutableArray(); 
        }
    }
}
