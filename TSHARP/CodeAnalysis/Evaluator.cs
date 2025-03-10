using System;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using TSharp.CodeAnalysis.Binding;
using TSharp.CodeAnalysis.Symbols;


namespace TSharp.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly ImmutableDictionary<FunctionSymbol, BoundBlockStatement> _functionBodies;
        private readonly BoundBlockStatement _root;
        private readonly Dictionary<VariableSymbol, object> _globals;
        private readonly Stack<Dictionary<VariableSymbol, object>> _locals = new Stack<Dictionary<VariableSymbol, object>>();
        private object _lastValue;
        private Random _random = null;


        public Evaluator(ImmutableDictionary<FunctionSymbol,BoundBlockStatement> functionBodies, BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _functionBodies = functionBodies;
            _root = root;
            _globals = variables;
        }

        public object Evaluate()
            => EvaluateStatement(_root);
        

        private object EvaluateStatement(BoundBlockStatement body)
        {
            var labelToIndex = new Dictionary<BoundLabel, int>();

            for (var i = 0; i < body.Statements.Length; i++)
            {
                if (body.Statements[i] is BoundLabelStatement l)
                    labelToIndex.Add(l.Label, i + 1);
            }

            var index = 0;

            while (index < body.Statements.Length)
            {
                var s = body.Statements[index];

                switch (s.Kind)
                {
                    case BoundNodeKind.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                        index++;
                        break;
                    case BoundNodeKind.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)s);
                        index++;
                        break;
                    case BoundNodeKind.GotoStatement:
                        var gs = (BoundGotoStatement)s;
                        index = labelToIndex[gs.Label];
                        break;
                    case BoundNodeKind.LabelStatement:
                        index++;
                        break;
                    case BoundNodeKind.ConditionalGotoStatement:
                        var cgs = (BoundConditionalGotoStatement)s;
                        var condition = (bool)EvaluateExpression(cgs.Condition);
                        if (condition && !cgs.JumpIfFalse
                        || !condition && cgs.JumpIfFalse)
                            index = labelToIndex[cgs.Label];
                        else
                            index++;
                        break;
                    default:
                        throw new Exception($"Unexcepted expression {s.Kind}");
                }
            }

            return _lastValue;
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _lastValue = value;
            Assign(node.Variable, value);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeKind.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)node);
                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);
                case BoundNodeKind.CallExpression:
                    return EvaluateCallExpression((BoundCallExpression) node);
                case BoundNodeKind.ConversionExpression:
                    return EvaluateConversionExpression((BoundConversionExpression)node);
                default:
                    throw new Exception($"Unexcepted expression {node.Kind}");
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression n)
            => n.Value;

        private object EvaluateVariableExpression(BoundVariableExpression v)
        {
            if (v.Variable.Kind == SymbolKind.GlobalVariable)
                return _globals[v.Variable];

            var locals = _locals.Peek();
            return locals[v.Variable];
        }
        
        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            Assign(a.Variable, value);
            return value;
        }


        private object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                default:
                    throw new Exception($"Unexcepted unary operator {u.Op}!");
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    if (b.Left.Type == TypeSymbol.String) // I will edit here
                        return (string)left + (string)right;
                    return (int)left + (int)right;
                case BoundBinaryOperatorKind.Substract:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);
                case BoundBinaryOperatorKind.Bigger:
                    return (int)left > (int)right;
                case BoundBinaryOperatorKind.Smaller:
                    return (int)left < (int)right;
                case BoundBinaryOperatorKind.EqualOreBigger:
                    return (int)left >= (int)right;
                case BoundBinaryOperatorKind.EqualOreSmaller:
                    return (int)left <= (int)right;
                default:
                    throw new Exception($"Unexcepted binary operator {b.Op}!");
            }
        }

        private object EvaluateCallExpression(BoundCallExpression node)
        {
            if (node.Function == BuildInFunctions.Input)
            {
                return Console.ReadLine();
            }
            else if (node.Function == BuildInFunctions.Print) 
            {
                var message = (string)EvaluateExpression(node.Arguments[0]);
                Console.WriteLine(message);
                return null;
            }
            else if (node.Function == BuildInFunctions.Random)
            {
                var max = (int)EvaluateExpression(node.Arguments[0])!;
                _random = _random ?? new Random();

                return _random.Next(max);
            }
            else
            {
                var locals = new Dictionary<VariableSymbol, object>();

                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    var parameter = node.Function.Parameters[i];
                    var value = EvaluateExpression(node.Arguments[i]);
                    locals.Add(parameter, value);
                }

                _locals.Push(locals);

                var statement = _functionBodies[node.Function];
                var result = EvaluateStatement(statement);

                _locals.Pop();

                return result;
            }
        }

        private object EvaluateConversionExpression(BoundConversionExpression node)
        {
            var value = EvaluateExpression(node.Expression);

            if (node.Type == TypeSymbol.Bool)
                return Convert.ToBoolean(value);
            
            if (node.Type == TypeSymbol.Int)
                return Convert.ToInt32(value);
        
            if (node.Type == TypeSymbol.String)
                return Convert.ToString(value);
            
            throw new Exception($"Unexcepted type {node.Type}");
        }
        
        private void Assign(VariableSymbol variable, object value)
        {
            if (variable.Kind == SymbolKind.GlobalVariable)
            {
                _globals[variable] = value;
            }
            else
            {
                var locals = _locals.Peek();
                locals[variable] = value;
            }
        }
    }
}
