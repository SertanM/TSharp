﻿using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Binding;
using System.Collections.Immutable;


namespace TSharp.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount;

        private Lowerer() {}

        private LabelSymbol GenerateLabel()
        {
            var name = $"Label{++_labelCount}";
            return new LabelSymbol(name);
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            var result = lowerer.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while(stack.Count > 0)
            {
                var current = stack.Pop();
                
                if(current is BoundBlockStatement block)
                    foreach (var s in block.Statements.Reverse())
                        stack.Push(s);
                else
                    builder.Add(current);
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatement == null)
            {
                var endLabel = GenerateLabel();
                var gotoFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, true);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(gotoFalse, node.ThenStatement, endLabelStatement));
                return RewriteStatement(result);
            }
            else
            {
                var elseLabel = GenerateLabel();
                var endLabel = GenerateLabel();

                var gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, true);
                var gotoEndStatement = new BoundGotoStatement(endLabel);
                var elseLabelStatement = new BoundLabelStatement(elseLabel);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoFalse, 
                    node.ThenStatement, 
                    gotoEndStatement,
                    elseLabelStatement,
                    node.ElseStatement,
                    endLabelStatement
                ));
                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var continueLabel = GenerateLabel();
            var checkLabel = GenerateLabel();
            var endLabel = GenerateLabel();
            var gotoCheck = new BoundGotoStatement(checkLabel);
            var continueLabelStatement = new BoundLabelStatement(continueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition, false);
            var endLabelStatement = new BoundLabelStatement(endLabel);

            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoCheck,
                    continueLabelStatement,
                    node.Statement,
                    checkLabelStatement,
                    gotoTrue,
                    endLabelStatement
                ));
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.StartExpression);
            var variableExpression = new BoundVariableExpression(node.Variable);
            
            var condition = new BoundBinaryExpression
                (
                    variableExpression,
                    BoundBinaryOperator.Bind(SyntaxKind.EqualOreSmallerToken, typeof(int), typeof(int)),
                    node.TargetExpression
                );

            var increment = new BoundExpressionStatement
                (
                    new BoundAssignmentExpression
                    (
                        node.Variable,
                        new BoundBinaryExpression
                        (
                            variableExpression,
                            BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int)),
                            new BoundLiteralExpression(1)
                        )
                    )
                );

            var whileBody = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, increment));
            var whileStatement = new BoundWhileStatement(condition, whileBody);
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(variableDeclaration, whileStatement));
            return RewriteStatement(result);
        }
    }
}