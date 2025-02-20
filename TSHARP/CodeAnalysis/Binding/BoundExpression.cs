namespace TSharp.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract System.Type Type { get; }
    }
}
