using ExtParser.Core;
using System;
using System.Threading.Tasks;

namespace ExtParser.Text
{
    /// <summary>
    /// Stream of characters.
    /// </summary>
    internal sealed class CharacterStream : ITokenStream<char>
    {
        /// <summary>
        /// Event that is raised when token is consumed.
        /// </summary>
        public event Action OnTokenConsumed;

        /// <summary>
        /// Code for special end-of-file character.
        /// </summary>
        public static readonly char EOF = '\u001A';

        /// <summary>
        /// Character data.
        /// </summary>
        private readonly string data;

        /// <summary>
        /// Gets current character position.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the current character from the input.
        /// </summary>
        public char CurrentToken
        {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterStream"/> class
        /// with the provided input data.
        /// </summary>
        /// <param name="data">Input data</param>
        public CharacterStream(string data)
            : this(data, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterStream"/> class
        /// with the provided input data and current position.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="position">Current character position</param>
        private CharacterStream(string data, int position)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            Position = position;
            CurrentToken = position < this.data.Length ? this.data[position] : EOF;
        }

        /// <summary>
        /// Adjusts the stream position to the next character.
        /// </summary>
        public Task Consume()
        {
            ++Position;

            CurrentToken =
                Position < data.Length
                    ? data[Position]
                    : EOF;

            OnTokenConsumed?.Invoke();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Create a copy of the current character stream.
        /// </summary>
        /// <returns>New instance of the <see cref="CharacterStream"/>.</returns>
        public ITokenStream<char> Clone()
        {
            return new CharacterStream(data, Position);
        }

        /// <summary>
        /// Shows current character with its surroundings.
        /// </summary>
        /// <returns>String that shows current position of the stream.</returns>
        public override string ToString()
        {
            const int PreviewRange = 10;

            var beforeStart = Math.Max(0, Position - PreviewRange);
            var beforeLength = Math.Min(PreviewRange, Position);

            var afterStart = Math.Min(Position + 1, data.Length - 1);
            var afterLength = Math.Min(PreviewRange, data.Length - Position - 1);

            var result = 
                (beforeStart == 0 ? null : "...")
                    + data.Substring(beforeStart, beforeLength)
                    + "[" + (CurrentToken == EOF ? "EOF" : new string(CurrentToken, 1)) + "]"
                    + data.Substring(afterStart, afterLength)
                    + (afterStart + afterLength >= data.Length ? null : "...");

            return FormatHelper.ToPrintable(result);
        }
    }
}
