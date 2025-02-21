using TSharp.CodeAnalysis.Symbols;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;

            switch (value)
            {
                case bool:
                    Type = TypeSymbol.Bool;
                    break;
                case int:
                    Type = TypeSymbol.Int;
                    break;
                case string:
                    Type = TypeSymbol.String;
                    break;
                default:
                    throw new Exception("There is no type for it");
            }
        }
        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public override TypeSymbol Type { get; }

        public object Value { get; }
    }
}
