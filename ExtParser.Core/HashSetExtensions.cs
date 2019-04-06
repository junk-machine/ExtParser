using System.Collections.Generic;

namespace ExtParser.Core
{
    /// <summary>
    /// Defines extension methods for <see cref="HashSet{T}"/> class.
    /// </summary>
    internal static class HashSetExtensions
    {
        /// <summary>
        /// Adds multiple elements to the hash set.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="target">Target hash set</param>
        /// <param name="elements">Elements to add</param>
        public static void AddRange<T>(this HashSet<T> target, IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                target.Add(element);
            }
        }
    }
}
