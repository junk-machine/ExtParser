using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Base class for all parser rules that need to match same rule repetitively.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal abstract class RepetitiveRuleBase<TToken> : IParserRule<TToken>
    {
        /// <summary>
        /// Gets the parser rule to match.
        /// </summary>
        protected IParserRule<TToken> Rule { get; private set; }

        /// <summary>
        /// Gets the name of the <see cref="RepetitiveRuleBase{TToken}"/> logical rule.
        /// </summary>
        public abstract string RuleName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepetitiveRuleBase{TToken}"/> class
        /// with the provided parser rule.
        /// </summary>
        /// <param name="rule">Parser rule to match</param>
        public RepetitiveRuleBase(IParserRule<TToken> rule)
        {
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        /// <summary>
        /// Matches tokens from the stream according to the implemented rule.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        public abstract Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context);

        /// <summary>
        /// Matches given rule zero or more times.
        /// </summary>
        /// <param name="branches">Branches to match repetitive rule on</param>
        /// <returns>All matching branches.</returns>
        protected async Task<IReadOnlyCollection<IParsingContext<TToken>>> MatchRepetitive(
            List<IParsingContext<TToken>> branches)
        {
            var outcomeBranches = new List<IParsingContext<TToken>>();

            var activeBranches =
                new List<Task<IReadOnlyCollection<IParsingContext<TToken>>>>(branches.Count);

            // Run loop until we don't get any new branches
            // TODO: We actually can add to branches created on a previous step,
            //       but this will generate enormous amount of overhead.
            //       Although we do expect all these other branches to die on the very
            //       next step, unless grammar has legitimate ambiguity.
            //       Need to perf-test both approaches and check the impact.
            while (branches.Count > 0)
            {
                outcomeBranches.Clear();
                outcomeBranches.AddRange(branches);

                branches.Clear();
                activeBranches.Clear();

                // Run given rule on all branches in parallel
                foreach (var branch in outcomeBranches)
                {
                    // All these matches are optional, so we need to clone the branch.
                    // Failing rules can consume some tokens and we won't be able to backtrack.
                    var branchTask = Rule.Match(branch.Clone());

                    if (activeBranches.Remove(branchTask))
                    {
                        // We got cached task, which means our new task is already completed
                        // with the same result as some other task.
                        // We can process results of both right now and don't need to store them.

                        var branchResult = await branchTask;

                        if (branchResult != null && branchResult.Count > 0)
                        {
                            // Since it is a cached task, we know that both branches completed with the same result.
                            branches.AddRange(branchResult);
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
                            branches.AddRange(branchResult);
                        }
                    }
                }
            }

            return outcomeBranches.Count > 0 ? outcomeBranches : null;
        }
    }
}
