using ExtParser.Text.GrammarParser.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExtParser.Text.GrammarParser
{
    /// <summary>
    /// Parse tree walker that generates parser code in C#.
    /// </summary>
    public sealed class ExtGrammarCodeGenWalker : TextParseTreeWalker
    {
        /// <summary>
        /// Information about the rule defined in the grammar.
        /// </summary>
        private sealed class RuleInfo
        {
            /// <summary>
            /// Name of the rule.
            /// </summary>
            public string Name;

            /// <summary>
            /// Flag that indicates whether rule is global.
            /// </summary>
            public bool IsGlobal;
        }

        /// <summary>
        /// Namespace where code should be generated.
        /// </summary>
        private readonly string namespaceName;

        /// <summary>
        /// Name of the grammar.
        /// </summary>
        private readonly string grammarName;

        /// <summary>
        /// Writer to print out generated code.
        /// </summary>
        private readonly TextWriter output;

        /// <summary>
        /// List of rules defined by the grammar.
        /// </summary>
        private readonly List<RuleInfo> rules;

        /// <summary>
        /// Set of rules that were referenced by any of the productions.
        /// </summary>
        /// <remarks>
        /// Rules that are not referenced in productions are considered entry-points and will get
        /// their own parse methods.
        /// </remarks>
        private readonly HashSet<string> referencedRules;

        /// <summary>
        /// Global rules that are excluded from the current rule.
        /// </summary>
        private List<string> excludedGlobalRules;

        /// <summary>
        /// Expression defining the body of the current rule.
        /// </summary>
        private ExpressionTreeNode ruleBodyExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtGrammarCodeGenWalker"/> class
        /// with the provided namespace name, grammar name and output writer.
        /// </summary>
        /// <param name="namespaceName">Namespace where code should be generated</param>
        /// <param name="grammarName">Name of the grammar</param>
        /// <param name="output">Writer to print out generated code</param>
        public ExtGrammarCodeGenWalker(string namespaceName, string grammarName, TextWriter output)
        {
            this.namespaceName =
                namespaceName ?? throw new ArgumentNullException(nameof(namespaceName));

            this.grammarName =
                grammarName ?? throw new ArgumentNullException(nameof(grammarName));

            this.output =
                output ?? throw new ArgumentNullException(nameof(output));

            rules = new List<RuleInfo>();
            referencedRules = new HashSet<string>();
        }

        /// <summary>
        /// Dispatches the logic based on the matched rule.
        /// </summary>
        /// <param name="match">Information about the matched rule</param>
        protected override void Enter(ITextParseTreeNode match)
        {
            switch (match.RuleName)
            {
                case ExtGrammarRules.Grammar:
                    EnterGrammar(match);
                    break;
                case ExtGrammarRules.Rule:
                    EnterRule(match);
                    break;
                case ExtGrammarRules.RuleName:
                    EnterRuleName(match);
                    break;
                case ExtGrammarRules.IsGlobalRule:
                    EnterIsGlobalRule(match);
                    break;
                case ExtGrammarRules.ExcludedGlobalRules:
                    EnterExcludedGlobalRules(match);
                    break;
                case ExtGrammarRules.RuleBody:
                    EnterRuleBody(match);
                    break;

                // Body rules
                case ExtGrammarRules.ExpressionGroup:
                    EnterSimpleExpression<SequenceExpression>(match);
                    break;
                case ExtGrammarRules.OptionalExpression:
                    EnterSimpleExpression<OptionalExpression>(match);
                    break;
                case ExtGrammarRules.ZeroOrMoreTimes:
                    EnterQuantifier<ZeroOrMoreTimesExpression>(match);
                    break;
                case ExtGrammarRules.OneOrMoreTimes:
                    EnterQuantifier<OneOrMoreTimesExpression>(match);
                    break;
                case ExtGrammarRules.AlternateExpression:
                    EnterAlternate(match);
                    break;
                case ExtGrammarRules.LiteralValue:
                    EnterLiteralValue(match);
                    break;
                case ExtGrammarRules.CharacterRange:
                    EnterCharacterRange(match);
                    break;

                // Default
                default:
                    base.Enter(match);
                    break;
            }
        }

        #region Rule body expressions

        /// <summary>
        /// Adds simple expression to the expression tree for the current rule.
        /// There are some expressions that don't require any special handling
        /// and just need to be added to the expression tree at the point where
        /// they are met in the parse tree.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterSimpleExpression<TExpression>(ITextParseTreeNode match)
            where TExpression : ExpressionTreeNode, new()
        {
            var newExpression = new TExpression();
            ruleBodyExpression.AddChild(newExpression);
            ruleBodyExpression = newExpression;

            base.Enter(match);

            ruleBodyExpression = ruleBodyExpression.Parent;
        }

        /// <summary>
        /// Adds quantifier expression (*, +) to the expression tree for the current rule.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterQuantifier<TExpression>(ITextParseTreeNode match)
            where TExpression : ExpressionTreeNode, new()
        {
            var previousExpression = ruleBodyExpression.RemoveLastChild();

            var quantifierExpression = new TExpression();
            quantifierExpression.AddChild(previousExpression);

            ruleBodyExpression.AddChild(quantifierExpression);
        }

        /// <summary>
        /// Adds alternate expression (Rule1 | Rule2) to the expression tree for the current rule.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterAlternate(ITextParseTreeNode match)
        {
            /* Alternate expressions are parsed like this:
             * 
             * Rule1 | Rule2 | Rule3
             * 
             * RuleName (Rule1)
             * AlternateExpression 
             * -RuleName (Rule2)
             * -AlternateExpression
             * --RuleName (Rule3)
             */

            if (ruleBodyExpression is OneOfExpression)
            {
                // Already alternating, just add to children
                base.Enter(match);
            }
            else
            {
                // Start new alternate with previous expression
                var previousExpression = ruleBodyExpression.RemoveLastChild();

                var alternateExpression = new OneOfExpression();
                alternateExpression.AddChild(previousExpression);

                ruleBodyExpression.AddChild(alternateExpression);
                ruleBodyExpression = alternateExpression;

                base.Enter(match);

                ruleBodyExpression = ruleBodyExpression.Parent;
            }
        }

        /// <summary>
        /// Adds literal value expression ("abc") to the expression tree for the current rule.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterLiteralValue(ITextParseTreeNode match)
        {
            ruleBodyExpression.AddChild(
                new LiteralExpression(match.GetImage()));
        }

        /// <summary>
        /// Adds character range expression ('x'..'y') to the expression tree for the current rule.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterCharacterRange(ITextParseTreeNode match)
        {
            if (match.Children.Count != 2)
            {
                throw new InvalidOperationException("Character range match is expected to have exactly two children");
            }

            ruleBodyExpression.AddChild(
                new CharacterRangeExpression(
                    match.Children[0].GetImage(),
                    match.Children[1].GetImage()));
        }

        #endregion Rule body expressions

        /// <summary>
        /// Generates rules namespace (usings block and rules classes).
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterGrammar(ITextParseTreeNode match)
        {
            output.WriteLine("namespace {0}.{1}.Rules", namespaceName, grammarName);
            output.WriteLine("{");

            output.WriteLine("using System.Collections.Generic;");
            output.WriteLine("using ExtParser.Core;");
            output.WriteLine("using ExtParser.Text;");

            base.Enter(match);

            output.WriteLine("}");

            WriteRuleNameConstants();
            WriteParser();
        }

        /// <summary>
        /// Generates a class with string constants that identify all parser rules in the grammar.
        /// </summary>
        private void WriteRuleNameConstants()
        {
            output.WriteLine("namespace {0}.{1}", namespaceName, grammarName);
            output.WriteLine("{");

            output.WriteLine("public sealed class {0}Rules", grammarName);
            output.WriteLine("{");

            foreach (var ruleInfo in rules)
            {
                output.WriteLine("public const string {0} = \"{0}\";", ruleInfo.Name);
            }

            output.WriteLine("}");

            output.WriteLine("}");
        }

        /// <summary>
        /// Generates parser namespace (usings block and a parser class stub).
        /// </summary>
        private void WriteParser()
        {
            output.WriteLine("namespace {0}.{1}", namespaceName, grammarName);
            output.WriteLine("{");

            output.WriteLine("using System.Threading;");
            output.WriteLine("using System.Threading.Tasks;");
            output.WriteLine("using ExtParser.Core;");
            output.WriteLine("using ExtParser.Text;");
            output.WriteLine("using {0}.{1}.Rules;", namespaceName, grammarName);

            output.WriteLine("public class {0}Parser : TextParser", grammarName);
            output.WriteLine("{");

            // Non-global rules that are not referenced in productions are considered entry-points.
            // Generate parse methods for all entry-point rules.
            foreach (var entryRule in rules.Where(r => !r.IsGlobal && !referencedRules.Contains(r.Name)))
            {
                output.WriteLine("public async Task<ITextParseTreeNode> Parse{0}(string input, CancellationToken cancellation)", entryRule.Name);
                output.WriteLine("{");
                output.WriteLine("return await Parse(input, ExtGrammarRules.{0}, cancellation);", entryRule.Name);
                output.WriteLine("}");
            }

            output.WriteLine();

            output.WriteLine("protected override IGrammar<char> DefineGrammar()");
            output.WriteLine("{");
            output.WriteLine("var grammar = new Grammar<char>();");

            foreach (var ruleInfo in rules)
            {
                output.WriteLine(
                    "grammar.AddRule(new {0}Rule(){1});",
                    ruleInfo.Name,
                    ruleInfo.IsGlobal ? ", isGlobal: true" : null);
            }

            output.WriteLine("return grammar;");
            output.WriteLine("}");

            output.WriteLine("}");

            output.WriteLine("}");
        }

        /// <summary>
        /// Generates rule class stub.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterRule(ITextParseTreeNode match)
        {
            output.Write("public partial class ");

            ruleBodyExpression = null;
            excludedGlobalRules = null;

            base.Enter(match);

            if (ruleBodyExpression == null)
            {
                // Rules without a body treated as terminals.
                // There are basically two types of rules - production and terminal. All productions
                // can be expressed through the grammar. If there was no body defined, then most likely
                // it was meant to be a light-weight terminal rule.
                output.WriteLine(" : TerminalParserRule<char>");
                output.WriteLine("{");

                WriteRuleNameProperty();
            }

            output.WriteLine("}");
        }

        /// <summary>
        /// Processes the rule name reference.
        /// Depending on the current parser walker context this can be:
        ///  1) new rule definition;
        ///  2) reference to an existing rule from excluded global rules list;
        ///  3) reference to an existing rule from the rule body.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterRuleName(ITextParseTreeNode match)
        {
            var ruleName = match.GetImage();

            if (match.Parent.RuleName == ExtGrammarRules.Rule)
            {
                rules.Add(new RuleInfo { Name = ruleName });

                output.Write("{0}Rule", ruleName);
            }
            else
            {
                referencedRules.Add(ruleName);

                if (match.Parent.RuleName == ExtGrammarRules.ExcludedGlobalRules)
                {
                    excludedGlobalRules.Add(ruleName);
                }
                else
                {
                    ruleBodyExpression.AddChild(new RuleExpression(ruleName));
                }
            }
        }

        /// <summary>
        /// Marks current rule as global.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterIsGlobalRule(ITextParseTreeNode match)
        {
            rules[rules.Count - 1].IsGlobal = true;
        }

        /// <summary>
        /// Resets the <see cref="excludedGlobalRules"/> list and visits all children elements
        /// that are expected to be excluded rule names.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterExcludedGlobalRules(ITextParseTreeNode match)
        {
            excludedGlobalRules = new List<string>();

            base.Enter(match);
        }

        /// <summary>
        /// Generates the contents (properties and methods) for the class that defines the parser rule.
        /// </summary>
        /// <param name="match">Current parse tree node</param>
        private void EnterRuleBody(ITextParseTreeNode match)
        {
            ruleBodyExpression = new SequenceExpression();

            output.WriteLine(" : TextProductionRule");
            output.WriteLine("{");

            WriteExcludedGlobalRules();

            WriteRuleNameProperty();

            output.WriteLine("protected override IParserRule<char> GetBody()");
            output.WriteLine("{");

            base.Enter(match);

            output.WriteLine("return");

            ruleBodyExpression.Visit(
                new CodeGenVisitor(grammarName, output));

            output.WriteLine(";");

            output.WriteLine("}");
        }

        /// <summary>
        /// Generates ExcludedGlobalRules class property.
        /// </summary>
        private void WriteExcludedGlobalRules()
        {
            if (excludedGlobalRules == null)
            {
                return;
            }
            else if (excludedGlobalRules.Count == 0)
            {
                output.WriteLine("protected override ISet<string> ExcludedGlobalRules { get { return AllGlobalRules; } }");
            }
            else
            {
                output.Write("private static HashSet<sstring> excludedGlobalRules = new HashSet<string> { ");

                var isFirst = true;
                foreach (var ruleName in excludedGlobalRules)
                {
                    if (!isFirst)
                    {
                        output.Write(", ");
                    }
                    else
                    {
                        isFirst = false;
                    }

                    output.Write("\"{0}\"", ruleName);
                }

                output.WriteLine("};");
                output.WriteLine("protected override ISet<string> ExcludedGlobalRules { get { return excludedGlobalRules; } }");
            }
        }

        /// <summary>
        /// Generates RuleName class property.
        /// </summary>
        private void WriteRuleNameProperty()
        {
            output.WriteLine(
                "public override string RuleName {{ get {{ return {0}Rules.{1}; }} }}",
                grammarName,
                rules[rules.Count - 1].Name);
        }
    }
}
