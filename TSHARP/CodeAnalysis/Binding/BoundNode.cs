using System.Reflection;


namespace TSharp.CodeAnalysis.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

        public IEnumerable<BoundNode> GetChilderen()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<BoundNode>)property.GetValue(this);
                    foreach (var child in children)
                        if (child != null)
                            yield return child;
                }
            }
        }

        public IEnumerable<(string Name, object Value)> GetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.Name == nameof(Kind)
                 || property.Name == nameof(BoundBinaryExpression.Op)
                 || property.Name == nameof(BoundVariableExpression.Variable))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType)
                 || typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                    continue;

                var value = property.GetValue(this);

                if (value != null)
                    yield return (property.Name,value);
            }
        }

        public void WriteTo(TextWriter writer)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Bound Tree");
            Console.ResetColor();
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            var marker = isLast ? "|__" : "|--";

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            writer.Write(indent);
            writer.Write(marker);
            if(isToConsole)
                Console.ForegroundColor = GetColor(node);

            var text = GetText(node);
            writer.Write(text);

            var isFirstProperty = true;

            foreach (var p in node.GetProperties()) 
            {
                if (isFirstProperty)
                    isFirstProperty = false;
                else
                {
                    if (isToConsole)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    writer.Write(",");
                }

                writer.Write(" ");

                if (isToConsole)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                writer.Write(p.Name);

                if (isToConsole)
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                writer.Write(" = ");

                if (isToConsole)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                writer.Write(p.Value);
            }

            if(isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;


            writer.WriteLine();

            indent += isLast ? "   " : "|  ";

            var last = node.GetChilderen().LastOrDefault();

            foreach (var child in node.GetChilderen())
                PrettyPrint(writer, child, indent, child == last);
        }

        private static ConsoleColor GetColor(BoundNode node)
        {
            if (node is BoundExpression)
                return ConsoleColor.Blue;

            if (node is BoundStatement)
                return ConsoleColor.Cyan;

            return ConsoleColor.Yellow;
        }

        private static string GetText(BoundNode node)
        {
            if (node is BoundBinaryExpression b)
                return b.Op.Kind.ToString() + "Expression";

            if (node is BoundUnaryExpression u)
                return u.Op.Kind.ToString() + "Expression";

            return node.Kind.ToString();
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
