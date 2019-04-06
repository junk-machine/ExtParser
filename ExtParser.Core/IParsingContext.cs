using System;
using System.Collections.Generic;

namespace ExtParser.Core
{
    /// <summary>
    /// Defines context for the parsing operation.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this parsing context understands.</typeparam>
    public interface IParsingContext<TToken>
    {
        /// <summary>
        /// Gets the identifier of the current context clone for tracking purposes.
        /// </summary>
        string BranchId { get; }

        /// <summary>
        /// Gets the parser grammar.
        /// </summary>
        IGrammar<TToken> Grammar { get; }

        /// <summary>
        /// Gets the token stream.
        /// </summary>
        ITokenStream<TToken> TokenStream { get; }

        /// <summary>
        /// Gets all rules that are being evaluated for the current token.
        /// </summary>
        Dictionary<IParserRule<TToken>, int> ActiveRules { get; }

        /// <summary>
        /// Gets the current parse tree.
        /// </summary>
        ParseTreeNode ParseTree { get; set; }

        /// <summary>
        /// Creates a copy of this parsing context instance with the same grammar,
        /// but cloned token stream.
        /// </summary>
        /// <returns>New instance of the <see cref="ParsingContext{TToken}"/></returns>
        IParsingContext<TToken> Clone();
    }
}
