﻿using TSharp.CodeAnalysis.Syntax;
using TSharp.CodeAnalysis.Binding;
using System.Collections.Immutable;
using TSharp.CodeAnalysis.Symbols;


namespace TSharp.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount;

        private Lowerer() {}

        private BoundLabel GenerateLabel()
        {
            var name = $"Label{++_labelCount}";
            return new BoundLabel(name);
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
            var checkLabel = GenerateLabel();

            var gotoCheck = new BoundGotoStatement(checkLabel);
            var continueLabelStatement = new BoundLabelStatement(node.ContinueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var gotoTrue = new BoundConditionalGotoStatement(node.ContinueLabel, node.Condition);
            var breakLabelStatement = new BoundLabelStatement(node.BreakLabel);

            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoCheck,
                    continueLabelStatement,
                    node.Statement,
                    checkLabelStatement,
                    gotoTrue,
                    breakLabelStatement
                ));
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.StartExpression);
            var variableExpression = new BoundVariableExpression(node.Variable);

            // I DONT WANT TO DIE

            var condition = new BoundBinaryExpression
                (
                    variableExpression,
                    BoundBinaryOperator.Bind(SyntaxKind.SmallerToken, TypeSymbol.Int, TypeSymbol.Int),
                    node.TargetExpression
                );

            var continueLabelStatement = new BoundLabelStatement(node.ContinueLabel);

            var increment = new BoundExpressionStatement
                (
                    new BoundAssignmentExpression
                    (
                        node.Variable,
                        new BoundBinaryExpression
                        (
                            variableExpression,
                            BoundBinaryOperator.Bind(SyntaxKind.PlusToken, TypeSymbol.Int, TypeSymbol.Int),
                            new BoundLiteralExpression(1)
                        )
                    )
                );

            var whileBody = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, continueLabelStatement, increment));
            var whileStatement = new BoundWhileStatement(condition, whileBody, node.BreakLabel, new BoundLabel("continue"));
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(variableDeclaration, whileStatement));
            return RewriteStatement(result);
        }
    }
}