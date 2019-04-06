using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ExtParser.Text.GrammarParser.Expressions
{
    internal sealed class CodeGenVisitor : IExpressionTreeVisitor
    {
        private readonly string grammarName;
        private readonly TextWriter output;

        private int indentLevel;

        public CodeGenVisitor(string grammarName, TextWriter output)
        {
            this.grammarName =
                grammarName ?? throw new ArgumentNullException(nameof(grammarName));

            this.output =
                output ?? throw new ArgumentNullException(nameof(output));
        }

        public void VisitCharacterRange(CharacterRangeExpression expression)
        {
            WriteLine("CharacterRange('{0}', '{1}')", expression.Min, expression.Max);
        }

        public void VisitLiteral(LiteralExpression expression)
        {
            WriteLine("Literal(\"{0}\")", expression.Value);
        }

        public void VisitOneOf(OneOfExpression expression)
        {
            WriteCall("OneOf", expression.Children);
        }

        public void VisitOneOrMoreTimes(OneOrMoreTimesExpression expression)
        {
            WriteCall("OneOrMoreTimes", expression.Children);
        }

        public void VisitOptional(OptionalExpression expression)
        {
            if (expression.Children.Count > 1)
            {
                WriteMethodStart("Optional");
                WriteCall("Sequence", expression.Children);
                WriteMethodEnd();
            }
            else if (expression.Children.Count == 1)
            {
                if (expression.Children[0] is OneOrMoreTimesExpression)
                {
                    // [ Rule+ ] can be optimized to Rule*
                    VisitZeroOrMoreTimes(expression.Children[0].Children);
                }
                else if (expression.Children[0] is ZeroOrMoreTimesExpression)
                {
                    // [ Rule* ] can be optimized to just Rule*
                    expression.Children[0].Visit(this);
                }
                else
                {
                    WriteCall("Optional", expression.Children);
                }
            }
        }

        public void VisitRule(RuleExpression expression)
        {
            WriteLine("Rule({0}Rules.{1})", grammarName, expression.RuleName);
        }

        public void VisitSequence(SequenceExpression expression)
        {
            if (expression.Children.Count > 1)
            {
                WriteCall("Sequence", expression.Children);
            }
            else if (expression.Children.Count == 1)
            {
                expression.Children[0].Visit(this);
            }
        }

        public void VisitZeroOrMoreTimes(ZeroOrMoreTimesExpression expression)
        {
            VisitZeroOrMoreTimes(expression.Children);
        }

        private void VisitZeroOrMoreTimes(IReadOnlyList<ExpressionTreeNode> childrenExpressions)
        {
            WriteCall("ZeroOrMoreTimes", childrenExpressions);
        }

        private void WriteCall(string methodName, IReadOnlyList<ExpressionTreeNode> children)
        {
            WriteMethodStart(methodName);

            for (var childIndex = 0; childIndex < children.Count; ++childIndex)
            {
                children[childIndex].Visit(this);

                if (childIndex < children.Count - 1)
                {
                    output.Write(new string(' ', indentLevel * 2));
                    output.Write(",");
                }
            }

            WriteMethodEnd();
        }

        private void WriteMethodStart(string methodName)
        {
            WriteLine(methodName + "(");
            ++indentLevel;
        }

        private void WriteMethodEnd()
        {
            --indentLevel;
            WriteLine(")");
        }

        private void WriteLine(string textFormat, params object[] args)
        {
            output.Write(new string(' ', indentLevel * 2));
            output.WriteLine(textFormat, args);
        }
    }
}
