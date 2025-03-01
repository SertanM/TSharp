
using System.Collections;
using System.Collections.Immutable;

namespace TSharp.CodeAnalysis.Syntax
{
    public abstract class SeperatedSyntaxList
    {
        public abstract ImmutableArray<SyntaxNode> GetWithSeparators();
    }

    public sealed class SeperatedSyntaxList<T> : SeperatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> _separatorsAndNodes;

        public SeperatedSyntaxList(ImmutableArray<SyntaxNode> separatorsAndNodes)
        {
            _separatorsAndNodes = separatorsAndNodes;
        }

        public int Count => (_separatorsAndNodes.Length + 1) / 2;

        public T this[int index] => (T) _separatorsAndNodes[index * 2];

        public SyntaxToken GetSeparators(int index) 
        {
            if (index == Count - 1)
                return null;
            return (SyntaxToken)_separatorsAndNodes[index * 2 + 1];
        }

        public override ImmutableArray<SyntaxNode> GetWithSeparators() => _separatorsAndNodes;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
