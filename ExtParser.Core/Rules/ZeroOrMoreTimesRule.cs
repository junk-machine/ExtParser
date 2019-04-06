using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Logical parser rule that matches given parser rule zero or more times.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal class ZeroOrMoreTimesRule<TToken> : RepetitiveRuleBase<TToken>
    {
        /// <summary>
        /// Gets the name of the <see cref="ZeroOrMoreTimesRule{TToken}{TToken}"/> logical rule.
        /// </summary>
        public override string RuleName => "{ZeroOrMoreTimes}";

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroOrMoreTimesRule{TToken}"/> class
        /// with the provided parser rule.
        /// </summary>
        /// <param name="rule">Parser rule to match</param>
        public ZeroOrMoreTimesRule(IParserRule<TToken> rule)
            : base(rule)
        {
        }

        /// <summary>
        /// Matches provided rule zero or more times.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>
        /// All possible parsing branches, if rule matches successfully, otherwise original parsing branch
        /// without any extra tokens consumed.
        /// </returns>
        public override async Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context)
        {
            return await MatchRepetitive(new List<IParsingContext<TToken>> { context });
        }

        /// <summary>
        /// Generates string representation of the current rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return Rule + "* ";
        }
    }
}
