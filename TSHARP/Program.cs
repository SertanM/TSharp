using static System.Net.Mime.MediaTypeNames;
using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Text;
using TSharp.CodeAnalysis;
using TSharp.CodeAnalysis.Symbols;

namespace TSharp
{
    internal static class Program 
    {
        private static void Main()
        {
            var repl = new TSharpRepl();
            repl.Run();
        }

        // I want to make it more than it
        // But I am gonna using only console to do this project
        // Because I realy don't want to deal with repl
        // Yeah I hate Repl

        //private static void Main(string[] args) 
        //{
        //    if (args.Length == 0)
        //    {
        //        Console.WriteLine("Usage: Ts <file_name>");
        //        return;
        //    }

        //    string filePath = args[0];

        //    if (!File.Exists(filePath))
        //    {
        //        Console.WriteLine($"File isn't exist: {filePath}");
        //        return;
        //    }

        //    string code = File.ReadAllText(filePath);

        //    var syntaxTree = SyntaxTree.Parse(code);

        //    var compilation = new Compilation(syntaxTree);

        //    var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

        //    var diagnostics = result.Diagnostics;


        //    if (!diagnostics.Any())
        //    {
        //        if (result.Value != null)
        //        {
        //            Console.ForegroundColor = ConsoleColor.Magenta;
        //            Console.WriteLine(result.Value);
        //            Console.ResetColor();
        //        }
        //    }
        //    else
        //    {
        //        foreach (var diagnostic in diagnostics)
        //        {
        //            var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
        //            var line = syntaxTree.Text.Lines[lineIndex];

        //            var lineNumber = lineIndex + 1;
        //            var character = diagnostic.Span.Start - line.Start + 1;

        //            Console.ForegroundColor = ConsoleColor.DarkRed;
        //            Console.Write($"({lineNumber}, {character}): ");
        //            Console.WriteLine(diagnostic);
        //            Console.ResetColor();

        //            var prefixSpan = TextSpan.Frombounds(line.Start, diagnostic.Span.Start);
        //            var suffixSpan = TextSpan.Frombounds(diagnostic.Span.End, line.End);

        //            var prefix = syntaxTree.Text.ToString(prefixSpan);
        //            var error = syntaxTree.Text.ToString(diagnostic.Span);
        //            var suffix = syntaxTree.Text.ToString(suffixSpan);

        //            Console.Write("    ");
        //            Console.Write(prefix);
        //            Console.ForegroundColor = ConsoleColor.DarkRed;
        //            Console.Write(error);
        //            Console.ResetColor();
        //            Console.Write(suffix);

        //            Console.WriteLine();
        //        }
        //    }

        //}
    }
}