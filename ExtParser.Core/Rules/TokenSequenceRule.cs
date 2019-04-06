using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core.Rules
{
    /// <summary>
    /// Parser rule that matches specified sequence of tokens.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    internal sealed class TokenSequenceRule<TToken> : ParserRule<TToken>
    {
        /// <summary>
        /// Expected sequence of tokens.
        /// </summary>
        private readonly TToken[] expectedSequence;

        /// <summary>
        /// Token equality comparer. 
        /// </summary>
        private readonly IEqualityComparer<TToken> tokenComparer;

        /// <summary>
        /// Gets the value indicating that rule is atomic.
        /// </summary>
        protected override bool IsAtomicRule => true;

        /// <summary>
        /// Gets the name of the <see cref="TokenSequenceRule{TToken}"/> rule.
        /// </summary>
        public override string RuleName => "{TokenSequence}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSequenceRule"/> class
        /// with the provided expected sequence and token equality comparer.
        /// </summary>
        /// <param name="expectedSequence">Expected sequence of tokens</param>
        /// <param name="tokenComparer">Token equality comparer</param>
        public TokenSequenceRule(TToken[] expectedSequence, IEqualityComparer<TToken> tokenComparer)
        {
            this.expectedSequence =
                expectedSequence ?? throw new ArgumentNullException(nameof(expectedSequence));

            this.tokenComparer =
                tokenComparer ?? throw new ArgumentNullException(nameof(tokenComparer));
        }

        /// <summary>
        /// Matches expected sequence of tokens.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>Original parsing context, if sequence successfully matches, otherwise null.</returns>
        protected override async Task<IReadOnlyCollection<IParsingContext<TToken>>> MatchInternal(IParsingContext<TToken> context)
        {
            TraceDebug(context, "Current token: {0}", context.TokenStream);

            for (var tokenIndex = 0; tokenIndex < expectedSequence.Length; ++tokenIndex)
            {
                if (tokenComparer.GetHashCode(expectedSequence[tokenIndex]) !=
                        tokenComparer.GetHashCode(context.TokenStream.CurrentToken)
                    || !tokenComparer.Equals(expectedSequence[tokenIndex], context.TokenStream.CurrentToken))
                {
                    return null;
                }

                await context.TokenStream.Consume();
            }

            return new[] { context };
        }

        public override string ToString()
        {
            return "\"" + FormatHelper.ToPrintable(expectedSequence) + "\"";
        }
    }
}
