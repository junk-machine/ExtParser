using System.Text;

namespace ExtParser.Core
{
    /// <summary>
    /// Defines helper methods to format different objects.
    /// </summary>
    public static class FormatHelper
    {
        /// <summary>
        /// Converts set of objects to a printable string.
        /// </summary>
        /// <param name="values">Objects to convert</param>
        /// <returns>Printable string, that represents objects.</returns>
        public static string ToPrintable<TElement>(params TElement[] values)
        {
            if (values == null || values.Length == 0)
            {
                return null;
            }

            var result = new StringBuilder();

            foreach (var value in values)
            {
                if (value == null)
                {
                    continue;
                }

                foreach (var character in value.ToString())
                {
                    if (character == '\r')
                    {
                    }
                    else if (character == '\n' || character == '\t')
                    {
                        result.Append(' ');
                    }
                    else if (char.IsControl(character))
                    {
                        result.AppendFormat("\\u{0:X2}", (int)character);
                    }
                    else
                    {
                        result.Append(character);
                    }
                }
            }

            return result.ToString();
        }
    }
}
