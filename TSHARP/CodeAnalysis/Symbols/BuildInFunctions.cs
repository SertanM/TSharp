using System.Collections.Immutable;
using System.Reflection;

namespace TSharp.CodeAnalysis.Symbols
{
    internal static class BuildInFunctions
    {
        public static readonly FunctionSymbol Print = new FunctionSymbol("output", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol Input = new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
        public static readonly FunctionSymbol Random = new FunctionSymbol("rnd", ImmutableArray.Create(new ParameterSymbol("max", TypeSymbol.Int)), TypeSymbol.Int);
        
        public static readonly FunctionSymbol CreateWindow = new FunctionSymbol("CreateWindow", ImmutableArray.Create(new ParameterSymbol("width", TypeSymbol.Int), new ParameterSymbol("height", TypeSymbol.Int), new ParameterSymbol("winName", TypeSymbol.String)), TypeSymbol.Void);
        public static readonly FunctionSymbol SetPixel = new FunctionSymbol("SetPixel", ImmutableArray.Create(new ParameterSymbol("x", TypeSymbol.Int), new ParameterSymbol("y", TypeSymbol.Int), new ParameterSymbol("R", TypeSymbol.Int), new ParameterSymbol("G", TypeSymbol.Int), new ParameterSymbol("B", TypeSymbol.Int)), TypeSymbol.Void);
        public static readonly FunctionSymbol IsWinClosed = new FunctionSymbol("IsWinClosed", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.Bool);
        public static readonly FunctionSymbol CloseWin = new FunctionSymbol("CloseWin", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.Void);
        public static readonly FunctionSymbol Render = new FunctionSymbol("Render", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.Void);


        internal static IEnumerable<FunctionSymbol> GetAll()
                            => typeof(BuildInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                       .Where(f => f.FieldType == typeof(FunctionSymbol))
                                                       .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}