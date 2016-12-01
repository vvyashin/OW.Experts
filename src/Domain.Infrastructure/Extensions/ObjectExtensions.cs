using System.Collections.Generic;

namespace Domain.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static IReadOnlyCollection<T> Enumerate<T>(this T @object)
        {
            return new[] {@object}.AsReadOnly();
        } 
    }
}
