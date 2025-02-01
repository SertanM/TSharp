using System;
using System.Collections.Immutable;

namespace Ts
{
    public sealed class Program 
    {
        static void Main(string[] args) 
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

                Console.ForegroundColor = ConsoleColor.DarkGray;

                if(showTree) PrettyPrint(syntaxTree.Root);

                if (!syntaxTree.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    var evalautor = new Evaluator(syntaxTree.Root);
                    var result = evalautor.Evaluate();
                    Console.WriteLine(result.ToString());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var diagnostic in syntaxTree.Diagnostics)
                        Console.WriteLine(diagnostic);

                    Console.WriteLine();
                }

                Console.ResetColor();
            }
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "|___" : "|---";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "|   ";

            var last = node.GetChilderen().LastOrDefault();

            foreach (var child in node.GetChilderen())
                PrettyPrint(child, indent, child == last);
        }
    }

    public enum SyntaxKind
    {
        NumberToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        MultiplyToken,
        DivisionToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression
    }

    public class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public override IEnumerable<SyntaxNode> GetChilderen()
            => Enumerable.Empty<SyntaxNode>();
        
    }

    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChilderen();
    }

    public abstract class ExpressionSyntax : SyntaxNode { }

    public sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;
        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChilderen()
        {
            yield return NumberToken;
        }
    }

    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }


        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public override IEnumerable<SyntaxNode> GetChilderen()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override IEnumerable<SyntaxNode> GetChilderen()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }

    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }

    public sealed class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current
        {
            get
            {
                if(_position >= _text.Length) 
                    return '\0';

                return _text[_position];
            }
        }

        private void Next()
            => _position++;
        

        public SyntaxToken NextToken()
        {
            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current)) 
                    Next();

                var length = _position - start;

                var text = _text.Substring(start, length);
                if(!int.TryParse(text, out var value))
                    _diagnostics.Add($"The number {_text} isn't valid Int32");

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current)) 
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if (Current == '+')
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            if (Current == '-')
                return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            if (Current == '*')
                return new SyntaxToken(SyntaxKind.MultiplyToken, _position++, "*", null);
            if (Current == '/')
                return new SyntaxToken(SyntaxKind.DivisionToken, _position++, "/", null);
            if(Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
            if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
            if (Current == '\0')
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position++, "\0", null);

            _diagnostics.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

    public sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
     
        private List<string> _diagnostics = new List<string>();
        private int _position;


        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken token;

            do
            {
                token = lexer.NextToken();
                if (token.Kind != SyntaxKind.WhiteSpaceToken
                && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }
        
        public IEnumerable<string> Diagnostic => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if(index >= _tokens.Length)
                return _tokens[_tokens.Length-1];

            return _tokens[index];
        }

        private SyntaxToken Current 
                         => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _diagnostics.Add($"ERROR: Unexcepted token <{Current.Kind}>, excepted {kind}");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        private ExpressionSyntax ParseExpression()
                              => ParseTerm();

        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (Current.Kind == SyntaxKind.PlusToken
                || Current.Kind == SyntaxKind.MinusToken)
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.MultiplyToken
                || Current.Kind == SyntaxKind.DivisionToken)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }
    }

    public sealed class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax root)
        {
            if (root is NumberExpressionSyntax n)
                return (int)n.NumberToken.Value;

            if (root is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            if(root is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                if (b.OperatorToken.Kind == SyntaxKind.MultiplyToken)
                    return left * right;
                if (b.OperatorToken.Kind == SyntaxKind.DivisionToken)
                    return left / right;

                throw new Exception($"Unexcepted operator {b.OperatorToken.Kind}!");
            }

            throw new Exception($"Unexcepted expression {root.Kind}");
        }
    }
}