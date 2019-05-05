using System.Collections.Generic;

namespace OW.Experts.Domain.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static IReadOnlyCollection<T> Enumerate<T>(this T @object)
        {
            return new[] { @object }.AsReadOnly();
        }
    }
}