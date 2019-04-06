namespace ExtParser.Core
{
    /// <summary>
    /// Defines context-free grammar in a form of parser rules.
    /// </summary>
    /// <typeparam name="TToken">Type of tokens this grammar is applicable to.</typeparam>
    public interface IGrammar<TToken>
    {
        /// <summary>
        /// Gets all global parser rules.
        /// </summary>
        /// <remarks>
        /// Global rules are the rules that can be matched anywhere, unless explicitly excluded.
        /// </remarks>
        IParserRule<TToken>[] GlobalRules { get; }

        /// <summary>
        /// Retrieves specified parser rule from the grammar.
        /// </summary>
        /// <param name="ruleName">Name of the parser rule</param>
        /// <returns>Requested parser rule.</returns>
        IParserRule<TToken> GetRule(string ruleName);
    }
}
