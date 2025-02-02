using System.Text;
using TSharp.CodeAnalysis;
using TSharp.CodeAnalysis.Binding;
using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Text;


namespace TSharp
{
    internal static class Program 
    {
        private static void Main() 
        {
            bool showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();
            Compilation previous = null;

            while (true) 
            {
                Console.ForegroundColor = ConsoleColor.Green;

                if (textBuilder.Length == 0)
                    Console.Write("> ");
                else
                    Console.Write(": ");

                Console.ResetColor();

                var input = Console.ReadLine();

                var isBlank = string.IsNullOrEmpty(input);

                if (textBuilder.Length == 0)
                {
                    if (string.IsNullOrEmpty(input))
                        break;

                    if (input == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }

                    if (input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine("Parse tree is " + (!showTree ? "not " : "") + "showing.");
                        continue;
                    }
                }

                textBuilder.Append(input);
                var text = textBuilder.ToString(); 
                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;
                

                var compilation = previous == null 
                                ? new Compilation(syntaxTree)
                                : previous.ContinueWith(syntaxTree);
                var result = compilation.Evaluate(variables);
                
                var diagnostics = result.Diagnostics;

                

                if (showTree) 
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }

                if (!diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                    previous = compilation;
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

                textBuilder.Clear();
                Console.ResetColor();
            }
        }

    }

}