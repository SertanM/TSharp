﻿using System.Collections.Immutable;
using System.Reflection;

namespace TSharp.CodeAnalysis.Symbols
{
    internal static class BuildInFunctions
    {
        public static readonly FunctionSymbol Print = new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol Input = new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
        public static readonly FunctionSymbol Random = new FunctionSymbol("rnd", ImmutableArray.Create(new ParameterSymbol("max", TypeSymbol.Int)), TypeSymbol.Int);

        internal static IEnumerable<FunctionSymbol> GetAll()
                            => typeof(BuildInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                       .Where(f => f.FieldType == typeof(FunctionSymbol))
                                                       .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}