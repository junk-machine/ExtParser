using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Parser rule that matches single token, if it falls within expected range.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal sealed class TokenRangeRule<TToken> : ParserRule<TToken>
    {
        /// <summary>
        /// Minimum token to match.
        /// </summary>
        private readonly TToken minValue;

        /// <summary>
        /// Maximum token to match.
        /// </summary>
        private readonly TToken maxValue;

        /// <summary>
        /// Token comparer.
        /// </summary>
        private readonly IComparer<TToken> tokenComparer;

        /// <summary>
        /// Gets the value indicating that rule is atomic.
        /// </summary>
        protected override bool IsAtomicRule => true;

        /// <summary>
        /// Gets the name of the <see cref="TokenRangeRule{TToken}"/> rule.
        /// </summary>
        public override string RuleName => "{TokenRange}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRangeRule{TToken}"/> class
        /// with the provided minimum, maximum values and token comparer.
        /// </summary>
        /// <param name="minValue">Minimum token value to match</param>
        /// <param name="maxValue">Maximum token value to match</param>
        /// <param name="tokenComparer">Token comparer</param>
        public TokenRangeRule(TToken minValue, TToken maxValue, IComparer<TToken> tokenComparer)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;

            this.tokenComparer = tokenComparer ?? throw new ArgumentNullException(nameof(tokenComparer));
        }

        /// <summary>
        /// Compares current token in the stream to the expected range.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>Original parsing context, if current token falls within the expected range, otherwise null.</returns>
        protected override async Task<IReadOnlyCollection<IParsingContext<TToken>>> MatchInternal(IParsingContext<TToken> context)
        {
            TraceDebug(context, "Current token: {0}", context.TokenStream);

            if (tokenComparer.Compare(context.TokenStream.CurrentToken, minValue) >= 0
                && tokenComparer.Compare(context.TokenStream.CurrentToken, maxValue) <= 0)
            {
                await context.TokenStream.Consume();
                return new[] { context };
            }

            return null;
        }

        /// <summary>
        /// Generates string representation of the current rule.
        /// </summary>
        /// <returns>String representation of the rule.</returns>
        public override string ToString()
        {
            return "'" + FormatHelper.ToPrintable(minValue) + "'..'" + FormatHelper.ToPrintable(maxValue) + "'";
        }
    }
}
