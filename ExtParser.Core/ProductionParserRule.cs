using ExtParser.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtParser.Core
{
    /// <summary>
    /// Base class for parser production rules.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    /// <remarks>
    /// Production rule is a parser rule that consists of references to other rules and matches by its body.
    /// </remarks>
    public abstract class ProductionParserRule<TToken> : ParserRule<TToken>
    {
        /// <summary>
        /// Cached body of this production rule.
        /// </summary>
        private readonly Lazy<IParserRule<TToken>> ruleBody;

        /// <summary>
        /// Logical value to indicate that none of the global rules should be matched
        /// when processing body of this production rule.
        /// </summary>
        protected static readonly ISet<string> AllGlobalRules = new HashSet<string>();

        /// <summary>
        /// Gets global rules that should not be matched within the body of this production rule.
        /// </summary>
        protected virtual ISet<string> ExcludedGlobalRules
        {
            get { return null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionParserRule{TToken}"/> class.
        /// </summary>
        public ProductionParserRule()
        {
            ruleBody = new Lazy<IParserRule<TToken>>(GetBody, true);
        }

        /// <summary>
        /// Matches the body of this production rule.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        protected sealed override Task<IReadOnlyCollection<IParsingContext<TToken>>> MatchInternal(IParsingContext<TToken> context)
        {
            return ruleBody.Value.Match(context);
        }

        /// <summary>
        /// Gets the body of this production parser rule.
        /// </summary>
        /// <returns>The body of this production parser rule.</returns>
        protected abstract IParserRule<TToken> GetBody();

        /// <summary>
        /// Creates logical parser rule that matches given rule zero or one time, making it optional.
        /// </summary>
        /// <param name="rule">Parser rule to match</param>
        /// <returns>New parser rule that implements optional match logic.</returns>
        protected IParserRule<TToken> Optional(IParserRule<TToken> rule)
        {
            return new OptionalRule<TToken>(rule);
        }

        /// <summary>
        /// Creates logical parser rule that matches zero or more occurences of the given rule.
        /// </summary>
        /// <param name="rule">Parser rule to match</param>
        /// <returns>New parser rule that implements zero-or-more-times logic.</returns>
        protected IParserRule<TToken> ZeroOrMoreTimes(IParserRule<TToken> rule)
        {
            return new ZeroOrMoreTimesRule<TToken>(rule);
        }

        /// <summary>
        /// Creates logical parser rule that matches one or more occurences of the given rule.
        /// </summary>
        /// <param name="rule">Parser rule to match</param>
        /// <returns>New parser rule that implements one-or-more-times logic.</returns>
        protected IParserRule<TToken> OneOrMoreTimes(IParserRule<TToken> rule)
        {
            return new OneOrMoreTimesRule<TToken>(rule);
        }

        /// <summary>
        /// Creates logical parser rule that tries to match multiple given parser rules in parallel
        /// and checks, if only one of them succeeds.
        /// </summary>
        /// <param name="rules">Parser rules to match</param>
        /// <returns>New parser rule that implements the one-of logic.</returns>
        protected IParserRule<TToken> OneOf(params IParserRule<TToken>[] rules)
        {
            return
                DecorateWithGlobalRules(
                    new OneOfRule<TToken>(rules));
        }

        /// <summary>
        /// Creates logical parser rule that matches multiple given parser rules in a sequnce.
        /// </summary>
        /// <param name="rules">Parser rules to match</param>
        /// <returns>New parser rule that implements the sequence matching logic.</returns>
        protected IParserRule<TToken> Sequence(params IParserRule<TToken>[] rules)
        {
            return new SequenceRule<TToken>(rules);
        }

        /// <summary>
        /// Creates new rule that matches specific sequence of tokens.
        /// </summary>
        /// <param name="expectedSequence">Expected sequence of tokens</param>
        /// <param name="tokenComparer">Token equality comparer</param>
        /// <returns>New rule that matches expected sequence of tokens.</returns>
        protected IParserRule<TToken> TokenSequence(
            TToken[] expectedSequence,
            IEqualityComparer<TToken> tokenComparer)
        {
            return
                DecorateWithGlobalRules(
                    new TokenSequenceRule<TToken>(expectedSequence, tokenComparer));
        }

        /// <summary>
        /// Creates new rule that matches a token from the specified range.
        /// </summary>
        /// <param name="minValue">Minimum token value to match</param>
        /// <param name="maxValue">Maximum token value to match</param>
        /// <param name="tokenComparer">Token comparer</param>
        /// <returns>New rule that matches token from the range.</returns>
        protected IParserRule<TToken> TokenRange(
            TToken minValue,
            TToken maxValue,
            IComparer<TToken> tokenComparer)
        {
            return
                DecorateWithGlobalRules(
                    new TokenRangeRule<TToken>(minValue, maxValue, tokenComparer));
        }

        /// <summary>
        /// Retieves specified parser rule from the grammar and decorates it with all necessary
        /// global parser rules.
        /// </summary>
        /// <param name="ruleName">Name of the parser rule</param>
        /// <typeparam name="TRule">Type of the parser rule to retrieve</typeparam>
        /// <returns>Requested parser rule preceeded by all required global rules.</returns>
        protected IParserRule<TToken> Rule(string ruleName)
        {
            return
                DecorateWithGlobalRules(
                    new RuleReferenceRule<TToken>(ruleName));
        }

        /// <summary>
        /// Decorates given parser rule with all necessary global parser rules.
        /// </summary>
        /// <param name="rule">Parser rule to decorate</param>
        /// <returns>Given parser rule decorated with all necessary global rules.</returns>
        private IParserRule<TToken> DecorateWithGlobalRules(IParserRule<TToken> rule)
        {
            return
                ExcludedGlobalRules == AllGlobalRules
                    ? rule
                    : new GlobalRulesDecoratorRule<TToken>(rule, ExcludedGlobalRules);
        }

        /// <summary>
        /// Generates a string representing this production rule.
        /// </summary>
        /// <returns>A string representing this production rule.</returns>
        public override string ToString()
        {
            return
                GetType().Name 
                    + GetExcludedGlobalRules()
                    + " = " 
                    + ruleBody.Value;
        }

        /// <summary>
        /// Generates a string that defines list of excluded global rules.
        /// </summary>
        /// <returns>String that defines list of excluded global rules.</returns>
        private string GetExcludedGlobalRules()
        {
            return
                ExcludedGlobalRules == null || ExcludedGlobalRules.Count == 0
                    ? null
                    : ExcludedGlobalRules == AllGlobalRules
                        ? " !"
                        : " !(" + string.Join(", ", ExcludedGlobalRules) + ")";
        }
    }
}
