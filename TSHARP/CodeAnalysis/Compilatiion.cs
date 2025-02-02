using TSharp.CodeAnalysis.Binding;
using TSharp.CodeAnalysis.Syntax;


namespace TSharp.CodeAnalysis
{
    public sealed class Compilatiion
    {
        public Compilatiion(SyntaxTree syntax)
        {
            Syntax = syntax;
        }

        public SyntaxTree Syntax { get; }
        public EvaluationResult Evaluate()
        {
            var binder = new Binder();
            var boundExpression = binder.BindExpression(Syntax.Root);

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();

            if(diagnostics.Any())
                return new EvaluationResult(diagnostics, null);


            var evaluator = new Evaluator(boundExpression);
            var value = evaluator.Evaluate();
            return new EvaluationResult(Array.Empty<Diagnostic>(), value);
        }
    }
}
