using System.Collections.Immutable;
using TSharp.CodeAnalysis.Lowering;
using TSharp.CodeAnalysis.Symbols;
using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Text;

namespace TSharp.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly FunctionSymbol _function;

        private Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> _loopStack = new();
        private BoundScope _scope;

        public Binder(BoundScope parent, FunctionSymbol function)
        {
            _scope = new BoundScope(parent);
            _function = function;

            if (function != null) 
            {
                foreach (var p in function.Parameters) 
                    _scope.TryDeclareVariable(p);
            }
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(parentScope, null);

            foreach (var function in syntax.Members.OfType<FunctionDeclarationSyntax>())
                binder.BindFunctionDeclaration(function);

            var statementBuilder = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach (var gloabalStatement in syntax.Members.OfType<GlobalStatementSyntax>())
            {
                var s = binder.BindStatement(gloabalStatement.Statement);
                statementBuilder.Add(s);
            }

            var statement = new BoundBlockStatement(statementBuilder.ToImmutable());

            var functions = binder._scope.GetDeclaredFunctions();
            var variables = binder._scope.GetDeclaredVariables();
            
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if(previous!=null) 
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics, variables, functions, statement);
        }

        public static BoundProgram BindProgram(BoundGlobalScope globalScope)
        {
            var parentScope = CreateParentScope(globalScope);

            var functionBodies = ImmutableDictionary.CreateBuilder<FunctionSymbol, BoundBlockStatement>();
            var diagnostics = new DiagnosticBag();

            var scope = globalScope;

            while (scope != null)
            {
                foreach (var function in scope.Functions)
                {
                    var binder = new Binder(parentScope, function);
                    var body = binder.BindStatement(function.Declaration.Body);
                    var loweredBody = Lowerer.Lower(body);
                    functionBodies.Add(function, loweredBody);

                    diagnostics.AddRange(binder.Diagnostics);
                }

                scope = scope.Previous;
            }
            var boundProgram = new BoundProgram(globalScope, diagnostics, functionBodies.ToImmutable());
            return boundProgram;
        }

        private void BindFunctionDeclaration(FunctionDeclarationSyntax syntax)
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();

            var seenParameters = new HashSet<string>();

            foreach (var parameterSyntax in syntax.Parameters)
            {
                var parameterName = parameterSyntax.Identifier.Text;
                var parameterType = BindTypeClause(parameterSyntax.Type);

                if (!seenParameters.Add(parameterName))
                {
                    _diagnostics.ReportSymbolAlreadyDeclared(parameterSyntax.Span, parameterName);
                }
                else
                {
                    var parameter = new ParameterSymbol(parameterName, parameterType);
                    parameters.Add(parameter);
                }
            }

            var type = BindTypeClause(syntax.Type) ?? TypeSymbol.Void;
            
            var function = new FunctionSymbol(syntax.Identifier.Text, parameters.ToImmutable(), type, syntax);
            if (!_scope.TryDeclareFunction(function))
                _diagnostics.ReportSymbolAlreadyDeclared(syntax.Identifier.Span, function.Name);
        }

        private static BoundScope CreateParentScope(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            var parent = CreateRootScope();

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);

                foreach (var f in previous.Functions)
                    scope.TryDeclareFunction(f);

                foreach (var v in previous.Variables)
                    scope.TryDeclareVariable(v);

                parent = scope;
            }
            return parent;
        }
        private static BoundScope CreateRootScope()
        {
            var result = new BoundScope(null);

            foreach (var f in BuildInFunctions.GetAll())
                result.TryDeclareFunction(f);

            return result;
        }
        public DiagnosticBag Diagnostics => _diagnostics;

        private BoundStatement BindErrorStatement()
        {
            return new BoundExpressionStatement(new BoundErrorExpression());
        }

        private BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                case SyntaxKind.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxKind.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                case SyntaxKind.BreakStatement:
                    return BindBreakStatement((BreakStatementSyntax)syntax);
                case SyntaxKind.ContinueStatement:
                    return BindContinueStatement((ContinueStatementSyntax)syntax);
                case SyntaxKind.ReturnStatement:
                    return BindReturnStatement((ReturnStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexcepted syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            _scope = new BoundScope(_scope);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            _scope = _scope.Parent;


            return new BoundBlockStatement(statements.ToImmutable());
        }
        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression, canBeVoid : true);
            return new BoundExpressionStatement(expression);
        }
        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
            var type = BindTypeClause(syntax.TypeClause);
            var initiliazier = BindExpression(syntax.Initiliazier);
            var variableType = type ?? initiliazier.Type;
            var convertedInitializier = BindConversion(syntax.Identifier.Span, initiliazier, variableType);
            var variable = BindVariable(syntax.Identifier, isReadOnly, variableType);

            return new BoundVariableDeclaration(variable, convertedInitializier);
        }
        private TypeSymbol BindTypeClause(TypeClauseSyntax syntax)
        {
            if (syntax == null)
                return null;

            var type = LookUpType(syntax.Identifier.Text);

            if (type == null)
                _diagnostics.ReportUndefinedType(syntax.Identifier.Span, syntax.Identifier.Text);

            return type;
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);
            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }
        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var statement = BindLoopBody(syntax.Statement, out var breakLabel, out var continueLabel);

            return new BoundWhileStatement(condition, statement, breakLabel, continueLabel);
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var startValue = BindExpression(syntax.StartValue, TypeSymbol.Int);

            _scope = new BoundScope(_scope);

            VariableSymbol variable = BindVariable(syntax.IdentifierToken, true, TypeSymbol.Int);

            var targetValue = BindExpression(syntax.TargetValue, TypeSymbol.Int);
            var body = BindLoopBody(syntax.Body, out var breakLabel, out var continueLabel);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, startValue, targetValue, body, breakLabel, continueLabel);
        }

        private BoundStatement BindLoopBody(StatementSyntax statement, out BoundLabel breakLabel, out BoundLabel continueLabel)
        {
            breakLabel = new BoundLabel("break");
            continueLabel = new BoundLabel("continue");

            _loopStack.Push((breakLabel, continueLabel));
            var boundBody = BindStatement(statement);
            _loopStack.Pop();

            return boundBody;
        }

        private BoundStatement BindBreakStatement(BreakStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Span, syntax.Keyword.Text);
                return BindErrorStatement();
            }

            var breakLabel = _loopStack.Peek().BreakLabel;

            return new BoundGotoStatement(breakLabel);
        }
        private BoundStatement BindContinueStatement(ContinueStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Span, syntax.Keyword.Text);
                return BindErrorStatement();
            }

            var continueLabel = _loopStack.Peek().ContinueLabel;
            return new BoundGotoStatement(continueLabel);
        }

        private BoundStatement BindReturnStatement(ReturnStatementSyntax syntax)
        {
            var expression = syntax.Expression == null ? null : BindExpression(syntax.Expression);

            if (_function == null)
            {
                _diagnostics.ReportInvalidReturn(syntax.ReturnKeyword.Span);
            }
            else
            {
                if (_function.Type == TypeSymbol.Void)
                {
                    if (expression != null)
                        _diagnostics.ReportInvalidReturnExpression(syntax.Expression.Span, _function.Name);
                }
                else
                {
                    if (expression == null)
                        _diagnostics.ReportMissingReturnExpression(syntax.ReturnKeyword.Span, _function.Type);
                    else
                        expression = BindConversion(syntax.Expression.Span, expression, _function.Type);
                }   
            }

            return new BoundReturnStatement(expression);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol targetType) => BindConversion(syntax, targetType);


        private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(syntax);

            if (!canBeVoid && result.Type == TypeSymbol.Void)
            {
                _diagnostics.ReportExpressionMustHaveValue(syntax.Span);
                return new BoundErrorExpression();
            }

            return result;
        }
        private BoundExpression BindExpressionInternal(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression: 
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.CallExpression:
                    return BindCallExpression((CallExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexcepted syntax {syntax.Kind}");
            }
        }
        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }
        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

            if (string.IsNullOrEmpty(name))
                return new BoundErrorExpression();
            

            if (!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundErrorExpression();
            }

            return new BoundVariableExpression(variable);
        }
        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if(!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return boundExpression;
            }

            if (variable.IsReadOnly)
                _diagnostics.ReportCannotAssign(syntax.EqualsToken.Span, name);

            var convertedExpression = BindConversion(syntax.Expression.Span, boundExpression, variable.Type);

            return new BoundAssignmentExpression(variable, convertedExpression);
        }
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);

            if (boundOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if(boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
                return new BoundErrorExpression();
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }
        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);

            if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return new BoundErrorExpression();
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
            var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();

            foreach (var arg in syntax.Arguments) 
            {
                var boundArgument = BindExpression(arg);
                boundArguments.Add(boundArgument);
            }
            
            if (!_scope.TryLookupFunction(syntax.Identifier.Text, out FunctionSymbol function))
            {
                if (syntax.Arguments.Count == 1
                 && LookUpType(syntax.Identifier.Text) is TypeSymbol type)
                    return BindConversion(syntax.Arguments[0], type, true);

                _diagnostics.ReportUndefinedFunction(syntax.Identifier.Span, syntax.Identifier.Text);
                return new BoundErrorExpression();
            }

            if (syntax.Arguments.Count != function.Parameters.Length)
            {
                _diagnostics.ReportWrongArgument(syntax.Span, function.Name, function.Parameters.Length, syntax.Arguments.Count);
                return new BoundErrorExpression();
            }

            for (int i = 0; i < syntax.Arguments.Count; i++)
            {
                var argument = boundArguments[i];
                var parameter = function.Parameters[i];

                if (parameter.Type != argument.Type)
                {
                    _diagnostics.ReportWrongArgumentType(syntax.Arguments[i].Span, function.Parameters[i].Name, parameter.Type, argument.Type);
                    return new BoundErrorExpression();
                }
            }

            return new BoundCallExpression(function, boundArguments.ToImmutableArray());
        }
        private BoundExpression BindConversion(ExpressionSyntax syntax, TypeSymbol type, bool allowExcpilit = false)
        {
            var expression = BindExpression(syntax);
            var diagnosticSpan = syntax.Span;

            return BindConversion(diagnosticSpan, expression, type, allowExcpilit);
        }
        private BoundExpression BindConversion(TextSpan diagnosticSpan, BoundExpression expression,  TypeSymbol type, bool allowExcpilit = false)
        {
            var conversion = Conversion.Classify(expression.Type, type);

            if (!conversion.Exist)
            {
                if (expression.Type != TypeSymbol.Error && type != TypeSymbol.Error)
                    _diagnostics.ReportCannotConvert(diagnosticSpan, expression.Type, type);

                return new BoundErrorExpression();
            }

            if (!allowExcpilit && conversion.IsExplicit) 
            {
                _diagnostics.ReportCannotConvertImplicitly(diagnosticSpan, expression.Type, type);
                return new BoundErrorExpression();
            }

            if (conversion.IsIdenity)
                return expression;

            return new BoundConversionExpression(type, expression);
        }
        private VariableSymbol BindVariable(SyntaxToken identifier, bool isReadonly, TypeSymbol symbol)
        {
            var name = identifier.Text ?? "?";
            var declare = !identifier.IsMissing;
            var variable = _function == null 
                                ? (VariableSymbol) new GlobalVariableSymbol(name, isReadonly, symbol)
                                : new LocalVariableSymbol(name, isReadonly, symbol);

            if (declare && !_scope.TryDeclareVariable(variable))
                _diagnostics.ReportSymbolAlreadyDeclared(identifier.Span, name);

            return variable;
        }
        private TypeSymbol LookUpType(string name)
        {
            switch (name)
            {
                case "bool":
                    return TypeSymbol.Bool;
                case "int":
                    return TypeSymbol.Int;
                case "string":
                    return TypeSymbol.String;
                default:
                    return null;
            }
        }
    }
}
