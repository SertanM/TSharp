namespace TSharp.CodeAnalysis.Syntax
{
    public sealed class FunctionDeclarationSyntax : MemberSyntax
    {
        public FunctionDeclarationSyntax(SyntaxToken functionKeyword, SyntaxToken identifier, SyntaxToken openParentheisesToken, SeperatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenthesisesToken, TypeClauseSyntax type, BlockStatementSyntax body)
        {
            FunctionKeyword = functionKeyword;
            Identifier = identifier;
            OpenParentheisesToken = openParentheisesToken;
            Parameters = parameters;
            CloseParenthesisesToken = closeParenthesisesToken;
            Type = type;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;

        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParentheisesToken { get; }
        public SeperatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParenthesisesToken { get; }
        public TypeClauseSyntax Type { get; }
        public BlockStatementSyntax Body { get; }
    }
}
