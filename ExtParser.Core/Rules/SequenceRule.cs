using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Logical parser rule that matches two or more rules sequentially.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal sealed class SequenceRule<TToken> : IParserRule<TToken>
    {
        /// <summary>
        /// Parser rules to match.
        /// </summary>
        private readonly IParserRule<TToken>[] rules;

        /// <summary>
        /// Gets the name of the <see cref="SequenceRule{TToken}"/> logical rule.
        /// </summary>
        public string RuleName => "{Sequence}";

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceRule{TToken}"/> class
        /// with the provided collecton of parser rules.
        /// </summary>
        /// <param name="rules">Parser rules to match</param>
        public SequenceRule(params IParserRule<TToken>[] rules)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <summary>
        /// Matches all provided parser rules sequentially.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if sequence matches successfully, otherwise null.</returns>
        public async Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context)
        {
            var outcomeBranches = new HashSet<IParsingContext<TToken>>();
            outcomeBranches.Add(context);

            var activeBranches =
                new List<Task<IReadOnlyCollection<IParsingContext<TToken>>>>();

            var succeededBranches = new List<IParsingContext<TToken>>();

            foreach (var rule in rules)
            {
                activeBranches.Clear();
                succeededBranches.Clear();

                // Run given rule on all branches in parallel
                foreach (var branch in outcomeBranches)
                {
                    var branchTask = rule.Match(branch);

                    if (activeBranches.Remove(branchTask))
                    {
                        // We got cached task, which means our new task is already completed
                        // with the same result as some other task.
                        // We can process results of both right now and don't need to store them.

                        var branchResult = await branchTask;

                        if (branchResult != null && branchResult.Count > 0)
                        {
                            // Since it is a cached task, we know that both branches completed with the same result.
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

                outcomeBranches.Clear();
                outcomeBranches.AddRange(succeededBranches);
            }

            return outcomeBranches.Count > 0 ? outcomeBranches : null;
        }

        /// <summary>
        /// Generates string representation of the current rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return String.Join(" ", rules.Select(r => r.ToString()));
        }
    }
}
