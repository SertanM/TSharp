namespace TSharp.CodeAnalysis
{
    internal sealed class LabelSymbol
    {
        internal LabelSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}