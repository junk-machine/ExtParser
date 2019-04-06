using System;
using System.Threading.Tasks;

namespace ExtParser.Core
{
    /// <summary>
    /// Defines a stream of tokens of type <typeparamref name="TToken"/>.
    /// </summary>
    /// <typeparam name="TToken">Type of tokens returned by the stream.</typeparam>
    public interface ITokenStream<TToken>
    {
        /// <summary>
        /// Event that is raised when token is consumed.
        /// </summary>
        event Action OnTokenConsumed;

        /// <summary>
        /// Gets the current token position in the stream.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the current token from the stream.
        /// </summary>
        TToken CurrentToken { get; }

        /// <summary>
        /// Consumes current token from the stream and adjust to the next one.
        /// </summary>
        Task Consume();

        /// <summary>
        /// Copies the state of the current stream.
        /// </summary>
        /// <returns>Copy of the current stream that can consume tokens independently.</returns>
        ITokenStream<TToken> Clone();
    }
}
