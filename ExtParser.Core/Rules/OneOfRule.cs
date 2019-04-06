using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Logical parser rule that matches one of the two or more rules.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal sealed class OneOfRule<TToken> : ParserRule<TToken>
    {
        /// <summary>
        /// Parser rules to match.
        /// </summary>
        private readonly IParserRule<TToken>[] rules;

        /// <summary>
        /// Gets the value indicating that rule is atomic.
        /// </summary>
        protected override bool IsAtomicRule => true;

        /// <summary>
        /// Gets the name of the <see cref="OneOfRule{TToken}"/> logical rule.
        /// </summary>
        public override string RuleName => "{OneOf}";

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOfRule{TToken}"/> class
        /// with the provided collection of parser rules.
        /// </summary>
        /// <param name="rules">Parser rules to match</param>
        public OneOfRule(IParserRule<TToken>[] rules)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <summary>
        /// Matches multiple parser rules in parallel and checks.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All matching parsing branches or null.</returns>
        protected override async Task<IReadOnlyCollection<IParsingContext<TToken>>> MatchInternal(
            IParsingContext<TToken> context)
        {
            var activeBranches =
                new List<Task<IReadOnlyCollection<IParsingContext<TToken>>>>(rules.Length);

            var succeededBranches =
                new List<IParsingContext<TToken>>();

            // Run lookahead on all branches in parallel
            for (var ruleIndex = 0; ruleIndex < rules.Length; ++ruleIndex)
            {
                var branchContext = context.Clone();

                TraceDebug(branchContext, "Branch is matching \"{0}\"", rules[ruleIndex]);

                // In case of "any-of" we only need to match global decorators once,
                // so lets hope this rule itself will be decorated, so it's safe
                // to unwrap all alternatives
                var branchTask = 
                    GlobalRulesDecoratorRule<TToken>
                        .Unwrap(rules[ruleIndex])
                        .Match(branchContext);

                if (activeBranches.Remove(branchTask))
                {
                    // We got cached task, which means our new task is already completed
                    // with the same result as some other task.
                    // We can process results of both right now and don't need to store them.

                    var branchResult = await branchTask;

                    if (branchResult != null && branchResult.Count > 0)
                    {
                        // Since it is a cached task, we know that both branches completed with the same result
                        succeededBranches.AddRange(branchResult);
                    }
                }
                else
                {
                    activeBranches.Add(branchTask);
                }
            }

            if (activeBranches.Count > 0)
            {
                var branchResults = await Task.WhenAll(activeBranches);

                for (var branchIndex = 0; branchIndex < branchResults.Length; ++branchIndex)
                {
                    var branchResult = branchResults[branchIndex];

                    if (branchResult != null && branchResult.Count > 0)
                    {
                        succeededBranches.AddRange(branchResult);
                    }
                }
            }

            if (succeededBranches.Count <= 0)
            {
                // None of the branches matched successfully
                return null;
            }

            return succeededBranches;
        }

        /// <summary>
        /// Generates string representation of the current rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return "( " + string.Join(" | ", rules.Select(r => r.ToString())) + " )";
        }
    }
}
