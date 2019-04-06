using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtParser.Core
{
    /// <summary>
    /// Defines single matching rule.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    public interface IParserRule<TToken>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        string RuleName { get; }

        /// <summary>
        /// Matches tokens from the stream according to the implemented rule.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context);
    }
}
