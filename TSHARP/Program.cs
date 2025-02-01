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
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);
                
                var diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();

                Console.ForegroundColor = ConsoleColor.DarkGray;

                if(showTree) PrettyPrint(syntaxTree.Root);

                if (!diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    var evalautor = new Evaluator(boundExpression);
                    var result = evalautor.Evaluate();
                    Console.WriteLine(result.ToString());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var diagnostic in diagnostics)
                        Console.WriteLine(diagnostic);

                    Console.WriteLine();
                }

                Console.ResetColor();
            }
        }

        private static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "|__" : "|--";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "   " : "|  ";

            var last = node.GetChilderen().LastOrDefault();

            foreach (var child in node.GetChilderen())
                PrettyPrint(child, indent, child == last);
        }
    }

}