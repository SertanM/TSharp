using TSharp.CodeAnalysis;
using TSharp.CodeAnalysis.Binding;
using TSharp.CodeAnalysis.Syntax;


namespace TSharp
{
    internal static class Program 
    {
        private static void Main() 
        {
            Console.ResetColor();
            bool showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();

            while (true) 
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if(string.IsNullOrEmpty(line)) 
                    return;

                if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine("Parse tree is " + (!showTree ? "not " : "") + "showing.");
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var compilatiion = new Compilatiion(syntaxTree);
                var result = compilatiion.Evaluate(variables);
                
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
                }
                else
                {

                    foreach (var diagnostic in diagnostics)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefix = line.Substring(0, diagnostic.Span.Start);
                        var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                        var suffix = line.Substring(diagnostic.Span.End);
                        Console.Write("    ");
                        Console.Write(prefix);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();
                        Console.Write(suffix);

                        Console.WriteLine();
                    }
                }

                Console.ResetColor();
            }
        }

    }

}