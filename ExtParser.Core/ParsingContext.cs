using System;
using System.Collections.Generic;
using System.Threading;

namespace ExtParser.Core
{
    /// <summary>
    /// Defines context for the parsing operation.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this parsing context understands.</typeparam>
    public sealed class ParsingContext<TToken> : IParsingContext<TToken>
    {
        /// <summary>
        /// Number of times this context was cloned.
        /// </summary>
        private int cloneIndex;

        /// <summary>
        /// Gets the identifier of the current context clone for tracking purposes.
        /// </summary>
        public string BranchId { get; private set; }

        /// <summary>
        /// Gets the parser grammar.
        /// </summary>
        public IGrammar<TToken> Grammar { get; private set; }

        /// <summary>
        /// Gets the token stream.
        /// </summary>
        public ITokenStream<TToken> TokenStream { get; private set; }

        /// <summary>
        /// Gets all rules that are being evaluated for the current token.
        /// </summary>
        public Dictionary<IParserRule<TToken>, int> ActiveRules { get; private set; }

        /// <summary>
        /// Gets the current parse tree.
        /// </summary>
        public ParseTreeNode ParseTree { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingContext{TToken}"/> class
        /// with the provided parser grammar and token stream.
        /// </summary>
        /// <param name="grammar">Parser grammar</param>
        /// <param name="tokenStream">Token stream</param>
        public ParsingContext(
            IGrammar<TToken> grammar,
            ITokenStream<TToken> tokenStream)
        {
            BranchId = ".";
            Grammar = grammar ?? throw new ArgumentNullException(nameof(grammar));
            TokenStream = tokenStream ?? throw new ArgumentNullException(nameof(tokenStream));

            ActiveRules = new Dictionary<IParserRule<TToken>, int>(3);
            
            TokenStream.OnTokenConsumed += ActiveRules.Clear;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingContext{TToken}"/> class
        /// with the provided clone identifier, parser grammar and token stream.
        /// </summary>
        /// <param name="cloneId">Clone identifier for this parser context</param>
        /// <param name="grammar">Parser grammar</param>
        /// <param name="tokenStream">Token stream</param>
        /// <param name="activeRules">All rules that are being evaluated for the current token</param>
        /// <param name="parseTree">Parse tree</param>
        private ParsingContext(
            string cloneId,
            IGrammar<TToken> grammar,
            ITokenStream<TToken> tokenStream,
            Dictionary<IParserRule<TToken>, int> activeRules,
            ParseTreeNode parseTree)
        {
            BranchId = cloneId;
            Grammar = grammar;
            TokenStream = tokenStream;
            ActiveRules = activeRules;
            ParseTree = parseTree;

            TokenStream.OnTokenConsumed += ActiveRules.Clear;
        }

        /// <summary>
        /// Creates a copy of this parsing context instance with the same grammar,
        /// but cloned token stream.
        /// </summary>
        /// <returns>New instance of the <see cref="ParsingContext{TToken}"/></returns>
        public IParsingContext<TToken> Clone()
        {
            var currentCloneIndex = Interlocked.Increment(ref cloneIndex);

            return
                new ParsingContext<TToken>(
                    BranchId + "/" + currentCloneIndex,
                    Grammar,
                    TokenStream.Clone(),
                    new Dictionary<IParserRule<TToken>, int>(ActiveRules),
                    ParseTree.Clone());
        }
    }
}
