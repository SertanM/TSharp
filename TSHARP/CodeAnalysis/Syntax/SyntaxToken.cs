﻿using TSharp.CodeAnalysis.Text;


namespace TSharp.CodeAnalysis.Syntax
{
    public sealed class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
            IsMissing = text == null;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public override TextSpan Span => new TextSpan(Position, Kind == SyntaxKind.EndOfFileToken ? 0 : Text?.Length ?? 0);

        public bool IsMissing { get; }
    }
}