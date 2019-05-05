namespace OW.Experts.Test.Infrastructure
{
    /// <summary>
    /// Contains extension method for setting properties with non public setters.
    /// </summary>
    public static class TestObjectExtension
    {
        /// <summary>
        /// Set property with protected setter.
        /// </summary>
        /// <typeparam name="T">Object type, for which the property is set.</typeparam>
        /// <typeparam name="TProp">Property type.</typeparam>
        /// <param name="object">Object, for which the property is set.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">Set value.</param>
        /// <returns>Object.</returns>
        /// <remarks>Property is set in intput object, and it return.</remarks>
        public static T SetProperty<T, TProp>(this T @object, string propertyName, TProp value)
        {
            typeof(T).GetProperty(propertyName).SetValue(@object, value);

            return @object;
        }
    }
}