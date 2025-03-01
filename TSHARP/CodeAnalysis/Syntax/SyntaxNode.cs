using System.Reflection;
using System.Text;
using TSharp.CodeAnalysis.Text;

namespace TSharp.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span 
        {
            get
            { 
                var first = GetChilderen().First().Span;
                var last = GetChilderen().Last().Span;
                return TextSpan.Frombounds(first.Start, last.End);
            }
        }

        public IEnumerable<SyntaxNode> GetChilderen()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties) 
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this);
                    if(child != null)
                        yield return child;
                }
                else if(typeof(SeperatedSyntaxList).IsAssignableFrom(property.PropertyType))
                {
                    var sepertatedSyntaxList = (SeperatedSyntaxList)property.GetValue(this);
                    foreach(var child in sepertatedSyntaxList.GetWithSeparators())
                        yield return child;
                }
            }
        }

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
                return token;

            return GetChilderen().Last().GetLastToken();
        }

        public void WriteTo(TextWriter writer)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Parse Tree");
            Console.ResetColor();
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            var marker = isLast ? "|__" : "|--";

            writer.Write(indent);

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            writer.Write(marker);

            if (isToConsole)
                Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;
            

            writer.Write(node.Kind);


            if (node is SyntaxToken t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            writer.WriteLine();

            indent += isLast ? "   " : "|  ";

            var last = node.GetChilderen().LastOrDefault();

            foreach (var child in node.GetChilderen())
                PrettyPrint(writer, child, indent, child == last);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
