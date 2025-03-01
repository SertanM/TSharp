using TSharp.CodeAnalysis.Symbols;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None     = new Conversion(false, false, false);
        public static readonly Conversion Identity = new Conversion(true, true, true);
        public static readonly Conversion Implicit = new Conversion(true, false, true);
        public static readonly Conversion Explicit  = new Conversion(true, false, false);


        private Conversion(bool exist, bool isIdenity, bool isImplicit)
        {
            Exist = exist;
            IsIdenity = isIdenity;
            IsImplicit = isImplicit;
        }

        public bool Exist { get; }
        public bool IsIdenity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exist && !IsImplicit;

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Conversion.Identity;

            if (from == TypeSymbol.Bool
             || from == TypeSymbol.Int)
                if (to == TypeSymbol.String)
                    return Conversion.Explicit;
            
            if (from == TypeSymbol.String)
                if (to == TypeSymbol.Int
                 || to == TypeSymbol.Bool)
                    return Conversion.Explicit;

            return Conversion.None;
        }
    }
}