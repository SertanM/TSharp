using TSharp.CodeAnalysis;
using TSharp.CodeAnalysis.Symbols;
using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Text;


namespace TSharp
{
    internal sealed class TSharpRepl : Repl
    {
        private Compilation _previous;
        private bool _showTree = false;
        private bool _showProgram = false;
        private readonly Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        protected override void RenderLine(string line)
        {
            var tokens = SyntaxTree.ParseTokens(line);
            
            var isWillMagenta = false;
            foreach (var token in tokens) 
            {
                var isMetaword = token.Text.StartsWith("#");
                var isKeyword = token.Kind.ToString().EndsWith("Keyword");
                var isNumber = token.Kind == SyntaxKind.NumberToken;
                var isIdentifier = token.Kind == SyntaxKind.IdentifierToken;

                if (isMetaword || isWillMagenta)
                {
                    isWillMagenta = !isWillMagenta;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                else if (isKeyword)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (isNumber)
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                else if (isIdentifier)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                Console.Write(token.Text);

                Console.ResetColor();
            }
        }


        protected override void EvaluateMetaCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            switch (input)
            {
                case "#showTree":
                    _showTree = !_showTree;
                    Console.WriteLine("Parse tree is " + (!_showTree ? "not " : "") + "showing.");
                    break;
                case "#showProgram":
                    _showProgram = !_showProgram;
                    Console.WriteLine("Bound tree is " + (!_showProgram ? "not " : "") + "showing.");
                    break;
                case "#cls":
                    Console.Clear();
                    break;
                case "#reset":
                    _previous = null;
                    Console.WriteLine("Scope is reseted.");
                    break;
                default:
                    base.EvaluateMetaCommand(input);
                    break;
            }
        }

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            var syntaxTree = SyntaxTree.Parse(text);

            if (syntaxTree.Root.Statement.GetLastToken().IsMissing)
                return false;

            return true;
        }

        protected override void EvaluateSubmission(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = _previous == null
                            ? new Compilation(syntaxTree)
                            : _previous.ContinueWith(syntaxTree);



            // The heart is a sea, I apologize to those who drown in it
            // The T is a freak, I apologize to everyone who killed in it


            if (_showTree)
                syntaxTree.Root.WriteTo(Console.Out);

            if (_showProgram)
                compilation.EmitTree(Console.Out);

            var result = compilation.Evaluate(_variables);

            var diagnostics = result.Diagnostics;


            if (!diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(result.Value);
                Console.ResetColor();
                _previous = compilation;
            }
            else
            {
                foreach (var diagnostic in diagnostics)
                {
                    var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                    var line = syntaxTree.Text.Lines[lineIndex];

                    var lineNumber = lineIndex + 1;
                    var character = diagnostic.Span.Start - line.Start + 1;

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write($"({lineNumber}, {character}): ");
                    Console.WriteLine(diagnostic);
                    Console.ResetColor();

                    var prefixSpan = TextSpan.Frombounds(line.Start, diagnostic.Span.Start);
                    var suffixSpan = TextSpan.Frombounds(diagnostic.Span.End, line.End);

                    var prefix = syntaxTree.Text.ToString(prefixSpan);
                    var error = syntaxTree.Text.ToString(diagnostic.Span);
                    var suffix = syntaxTree.Text.ToString(suffixSpan);

                    Console.Write("    ");
                    Console.Write(prefix);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(error);
                    Console.ResetColor();
                    Console.Write(suffix);

                    Console.WriteLine();
                }
            }
        }

    }
}