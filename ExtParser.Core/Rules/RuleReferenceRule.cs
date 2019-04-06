using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Parser rule that calls another parser rule from the grammar.
    /// </summary>
    /// <typeparam name="TRule">Type of the rule to call.</typeparam>
    /// <typeparam name="TToken">Type of the tokens referenced rule matches.</typeparam>
    internal sealed class RuleReferenceRule<TToken> : IParserRule<TToken>
    {
        /// <summary>
        /// Name of the referenced rule.
        /// </summary>
        private readonly string ruleName;

        /// <summary>
        /// Gets the name of the <see cref="RuleReferenceRule{TToken}"/> logical rule.
        /// </summary>
        public string RuleName => "{" + ruleName + "}";

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleReferenceRule"/> class
        /// with the provided rule name.
        /// </summary>
        /// <param name="ruleName">Name of the referenced rule</param>
        public RuleReferenceRule(string ruleName)
        {
            this.ruleName = ruleName ?? throw new ArgumentNullException(nameof(ruleName));
        }

        /// <summary>
        /// Retrieves referenced rule from the grammar and tries matches it.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        public Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context)
        {
            return context.Grammar.GetRule(ruleName).Match(context);
        }

        /// <summary>
        /// Generates string representation of the current rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return ruleName;
        }
    }
}
