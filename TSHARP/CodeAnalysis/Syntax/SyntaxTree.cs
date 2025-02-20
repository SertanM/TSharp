
using System.Collections.Immutable;
using TSharp.CodeAnalysis.Text;

namespace TSharp.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostic.ToImmutableArray();

            Text = text;
            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
                                            => ParseTokens(SourceText.From(text));
        


        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
        {
            var lexer = new Lexer(text);

            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;

                yield return token;
            }
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }
    }
}
