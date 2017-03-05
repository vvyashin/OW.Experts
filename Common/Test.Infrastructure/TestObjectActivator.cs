using System;

namespace Test.Infrastructure
{
    /// <summary>
    /// Contains method for creating instances of types with non public constructor
    /// </summary>
    public static class TestObjectActivator
    {
        /// <summary>
        /// Create instance of type with non public parametless constructor
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>New instance of type T</returns>
        public static T Create<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof (T), true);
        }
    }
}
