namespace TSharp.CodeAnalysis.Syntax
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(SyntaxToken forKeyword, SyntaxToken identifierToken, SyntaxToken equalToken, ExpressionSyntax startValue, 
            SyntaxToken toKeyword, ExpressionSyntax targetValue, StatementSyntax statement)
        {
            ForKeyword = forKeyword;
            IdentifierToken = identifierToken;
            EqualToken = equalToken;
            StartValue = startValue;
            ToKeyword = toKeyword;
            TargetValue = targetValue;
            Body = statement;
        }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;

        public SyntaxToken ForKeyword { get; }
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualToken { get; }
        public ExpressionSyntax StartValue { get; }
        public SyntaxToken ToKeyword { get; }
        public ExpressionSyntax TargetValue { get; }
        public StatementSyntax Body { get; }
    }
}
