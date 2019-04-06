using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Logical parser rule that matches given parser rule zero or one time,
    /// making the rule optional.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal sealed class OptionalRule<TToken> : IParserRule<TToken>
    {
        /// <summary>
        /// Parser rule to match.
        /// </summary>
        private readonly IParserRule<TToken> rule;

        /// <summary>
        /// Gets the name of the <see cref="OptionalRule{TToken}"/> logical rule.
        /// </summary>
        public string RuleName => "{Optional}";

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalRule{TToken}"/> class
        /// with the provided parser rule.
        /// </summary>
        /// <param name="rule">Parser rule to match</param>
        public OptionalRule(IParserRule<TToken> rule)
        {
            this.rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        /// <summary>
        /// Tries to match provided rule one time.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>
        /// Original branch, if rule doesn't match, otherwise both - original branch and
        /// branch with consumed optional match.
        /// </returns>
        public async Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context)
        {
            var branchesWithOptionalMatch = await rule.Match(context.Clone());

            if (branchesWithOptionalMatch != null && branchesWithOptionalMatch.Count > 0)
            {
                return
                    new List<IParsingContext<TToken>>(branchesWithOptionalMatch)
                    {
                        context
                    };
            }

            // Optional rule didn't match, return original parsing context
            return new[] { context };
        }

        /// <summary>
        /// Generates string representation of the rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return "[ " + rule + " ]";
        }
    }
}
