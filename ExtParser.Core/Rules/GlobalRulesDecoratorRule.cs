using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Parser rule that matches necessary global rules before the actual parser rule.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens referenced rule matches.</typeparam>
    internal sealed class GlobalRulesDecoratorRule<TToken> : IParserRule<TToken>
    {
        /// <summary>
        /// Parser rule to decorate.
        /// </summary>
        private IParserRule<TToken> rule;

        /// <summary>
        /// Set of excluded global rules.
        /// </summary>
        private ISet<string> excludedGlobalRules;

        /// <summary>
        /// Gets the name of the <see cref="GlobalRulesDecoratorRule{TToken}"/> logical rule.
        /// </summary>
        public string RuleName => "{GlobalRulesDecorator}";

        /// <summary>
        /// Intializes a new instance of the <see cref="GlobalRulesDecoratorRule{TToken}"/> class
        /// with the provided underlying rule and collection of excluded global rules.
        /// </summary>
        /// <param name="rule">Underlying parser rule</param>
        /// <param name="excludedGlobalRules">Set of excluded global rules</param>
        public GlobalRulesDecoratorRule(
            IParserRule<TToken> rule,
            ISet<string> excludedGlobalRules)
        {
            this.rule =
                rule ?? throw new ArgumentNullException(nameof(rule));

            this.excludedGlobalRules = excludedGlobalRules;
        }

        /// <summary>
        /// Matches decorated parser rule preceeded by necessary global rules.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        public Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context)
        {
            var globalRules =
                excludedGlobalRules == null || excludedGlobalRules.Count == 0
                    ? context.Grammar.GlobalRules
                    : context.Grammar.GlobalRules.Where(IsIncludedGlobalRule).ToArray();

            if (globalRules.Length > 0)
            {
                var globalRulesRule =
                    new ZeroOrMoreTimesRule<TToken>(
                        globalRules.Length == 1
                            ? globalRules[0]
                            : new OneOfRule<TToken>(globalRules));

                return new SequenceRule<TToken>(globalRulesRule, rule).Match(context);
            }
            else
            {
                return rule.Match(context);
            }
        }

        /// <summary>
        /// Checks, if specified global <paramref name="rule"/> should be included in matching.
        /// </summary>
        /// <param name="rule">Global rule to check</param>
        /// <returns>true if rule should be included, otherwise false.</returns>
        private bool IsIncludedGlobalRule(IParserRule<TToken> rule)
        {
            return !excludedGlobalRules.Contains(rule.RuleName);
        }

        /// <summary>
        /// Extracts underlying parser rule from the given decorator.
        /// </summary>
        /// <param name="rule">Potential decorator rule</param>
        /// <returns>Underlying parser rule or original rule, if given rule is not decorated.</returns>
        public static IParserRule<TToken> Unwrap(IParserRule<TToken> rule)
        {
            var decoratedRule = rule as GlobalRulesDecoratorRule<TToken>;

            return
                decoratedRule == null
                    ? rule
                    : decoratedRule.rule;
        }

        /// <summary>
        /// Generates string representation of the current rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return rule.ToString();
        }
    }
}
