using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Logical parser rule that matches given parser rule one or more times.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal sealed class OneOrMoreTimesRule<TToken> : RepetitiveRuleBase<TToken>
    {
        /// <summary>
        /// Gets the name of the <see cref="OneOrMoreTimesRule{TToken}"/> logical rule.
        /// </summary>
        public override string RuleName => "{OneOrMore}";

        /// <summary>
        /// Initializes a new instance of the <see cref="OneOrMoreTimesRule{TToken}"/> class
        /// with the provided parser rule.
        /// </summary>
        /// <param name="rule">Parser rule to match</param>
        public OneOrMoreTimesRule(IParserRule<TToken> rule)
            : base(rule)
        {
        }

        /// <summary>
        /// Matches provided rule one or more times.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        public override async Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context)
        {
            var result = await Rule.Match(context);
            
            if (result == null || result.Count <= 0)
            {
                return null;
            }

            return await MatchRepetitive(new List<IParsingContext<TToken>>(result));
        }

        /// <summary>
        /// Generates string representation of the rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return Rule + "+ ";
        }
    }
}
