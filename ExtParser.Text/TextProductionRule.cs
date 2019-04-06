using ExtParser.Core;
using System.Collections.Generic;

namespace ExtParser.Text
{
    /// <summary>
    /// Base class for parser rules that operate over character tokens.
    /// </summary>
    public abstract class TextProductionRule : ProductionParserRule<char>
    {
        /// <summary>
        /// Creates new rule that matches specified literal value.
        /// </summary>
        /// <param name="value">Literal value to match</param>
        /// <returns>New parser rule that matches specified literal value.</returns>
        protected IParserRule<char> Literal(string value)
        {
            return
                TokenSequence(
                    value.ToCharArray(),
                    CharEqualityComparer.CaseInsensitive);
        }

        /// <summary>
        /// Creates new rule that matches character from the specified range.
        /// </summary>
        /// <param name="minValue">Minimum character value to match</param>
        /// <param name="maxValue">Maximum character value to match</param>
        /// <returns>New parser rule that matches character from the specified range.</returns>
        protected IParserRule<char> CharacterRange(char minValue, char maxValue)
        {
            return TokenRange(minValue, maxValue, Comparer<char>.Default);
        }
    }
}
