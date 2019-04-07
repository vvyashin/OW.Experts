using System;

namespace Domain.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Retrieves a first line of this instance.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FirstLine(this string str)
        {
            return str.Substring(Environment.NewLine);
        }

        /// <summary>
        /// Retrieves a substring of this instance. The substring starts at first character and ends at index of specified string value.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value">substring, which index is last index of return value</param>
        /// <returns></returns>
        public static string Substring(this string str, string value)
        {
            return str.Substring(0, str.IndexOf(value));
        }
    }
}
