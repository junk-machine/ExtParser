using System.Collections.Generic;

namespace ExtParser.Text
{
    /// <summary>
    /// Character equality comparer.
    /// </summary>
    internal sealed class CharEqualityComparer : EqualityComparer<char>
    {
        /// <summary>
        /// Case-sensitive character equality comparer.
        /// </summary>
        public static IEqualityComparer<char> CaseSensitive = new CharEqualityComparer(true);

        /// <summary>
        /// Case-insensitive character equality comparer.
        /// </summary>
        public static IEqualityComparer<char> CaseInsensitive = new CharEqualityComparer(false);

        /// <summary>
        /// Flag that indicates whether to perform case-sensitive comparison.
        /// </summary>
        private bool caseSensitive;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharEqualityComparer"/> class
        /// with the provided case-sensitivity flag.
        /// </summary>
        /// <param name="caseSensitive">Whether to perform case-sensitive comparison</param>
        private CharEqualityComparer(bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
        }

        /// <summary>
        /// Compares two characters.
        /// </summary>
        /// <param name="x">First character</param>
        /// <param name="y">Second character</param>
        /// <returns>true if characters are equal, otherwise false.</returns>
        public override bool Equals(char x, char y)
        {
            return
                caseSensitive
                    ? x == y
                    : char.ToLowerInvariant(x) == char.ToLowerInvariant(y);
        }

        /// <summary>
        /// Computes hash code for the given character.
        /// </summary>
        /// <param name="obj">Character to compute hash for</param>
        /// <returns>Character hash code.</returns>
        public override int GetHashCode(char obj)
        {
            return
                caseSensitive
                    ? obj.GetHashCode()
                    : char.ToLowerInvariant(obj).GetHashCode();
        }
    }
}
