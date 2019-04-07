using System;
using System.Collections.Generic;

namespace OW.Experts.Domain.Infrastructure.Extensions
{
    public static class ArrayExtensions
    {
        public static IReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            return Array.AsReadOnly(array);
        }
    }
}
