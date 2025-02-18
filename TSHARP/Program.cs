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
            var variables = new Dictionary<VariableSymbol, object>();
            Compilation previous = null;
            var textBuilder = new StringBuilder();

            var showTree = false;
            var showProgram = false;

            Console.ResetColor();

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

                    if(input == "#showProgram")
                    {
                        showProgram = !showProgram;
                        Console.WriteLine("Bound tree is " + (!showProgram ? "not " : "") + "showing.");
                        continue;
                    }

                    if(input == "#reset")
                    {
                        previous = null;
                        Console.WriteLine("Scope is reseted.");
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


                
                // The heart is a sea, I apologize to those who drown in it

                if (showTree) 
                    syntaxTree.Root.WriteTo(Console.Out);
                
                if (showProgram)
                    compilation.EmitTree(Console.Out);

                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;


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