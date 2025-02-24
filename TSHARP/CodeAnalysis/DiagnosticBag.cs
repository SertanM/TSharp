﻿using System.Collections;
using System.Xml.Linq;
using TSharp.CodeAnalysis.Symbols;
using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Text;

namespace TSharp.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator()
                                    => _diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
                                    => GetEnumerator();

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }

        private void Report(TextSpan span, string message)
        {
            var diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        

        public void ReportInvalidNumber(TextSpan textSpan, string text, TypeSymbol type)
        {
            var message = $"The number {text} isn't valid {type}";
            Report(textSpan, message);
        }

        public void ReportBadToken(int position, char character)
        {
            var span = new TextSpan(position, 1);
            var message = $"Bad character input: '{character}'";
            Report(span, message);
        }

        public void ReportUnterminatedString(TextSpan span)
        {
            var message = "Unterminated string literal.";
            Report(span, message);
        }

        public void ReportUnexceptedToken(TextSpan span, SyntaxKind currentKind, SyntaxKind exceptedKind)
        {
            var message = $"Unexcepted token {currentKind}, excepted {exceptedKind}";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for '{operandType}'";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"Variable {name} doesn't exist";
            Report(span, message);
        }

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"Variable {name} is already exist";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert '{fromType}' to '{toType}'";
            Report(span, message);
        }

        public void ReportCannotAssign(TextSpan span, string name)
        {
            var message = $"Variable '{name}' is readonly and cannot be assigned to";
            Report(span, message);
        }

        
    }
}
