using System;
using JetBrains.Annotations;

namespace OW.Experts.Domain.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Retrieves a first line of provided string.
        /// </summary>
        /// <param name="str">String to retrieve a first line.</param>
        /// <returns>A substring of <paramref name="str"/> that begins at first character and ends at first NewLine symbol.</returns>
        public static string FirstLine(this string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            return str.Substring(Environment.NewLine);
        }

        /// <summary>
        /// Retrieves a substring of provided string.
        /// </summary>
        /// <param name="str">String to retrieve substring.</param>
        /// <param name="value">substring, which index is last index of return value.</param>
        /// <returns>A substring of <paramref name="str"/> that begins at first character and ends at index of specified string value.</returns>
        public static string Substring([NotNull] this string str, [NotNull] string value)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (value == null) throw new ArgumentNullException(nameof(value));

            return str.Substring(0, str.IndexOf(value));
        }
    }
}