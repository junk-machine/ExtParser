using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core
{
    /// <summary>
    /// Base class for terminal parser rules.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    /// <remarks>
    /// Terminal parser rule is a rule that doesn't have references to any other rules
    /// and only matches specific set of tokens.
    /// </remarks>
    public abstract class TerminalParserRule<TToken> : ParserRule<TToken>
    {
        /// <summary>
        /// Matches specific set of tokens.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>Original context with consumed tokens or null, if match did not succeed.</returns>
        protected sealed override async Task<IReadOnlyCollection<IParsingContext<TToken>>> MatchInternal(
            IParsingContext<TToken> context)
        {
            if (await TryMatch(context))
            {
                return new[] { context };
            }

            return null;
        }

        /// <summary>
        /// Consumes specific set of tokens from the stream.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>true if tokens were successfully consumed, otherwise false.</returns>
        protected abstract Task<bool> TryMatch(IParsingContext<TToken> context);
    }
}
