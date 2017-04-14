namespace Kingo
{
    /// <summary>
    /// Contains extension methods for every object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// A method that simply ignores whatever is passed to it. The purpose of this method is to provide a clean syntax and increase code readability
        /// in test methods where return values of methods are to be ignored, but require consumption of it by the compiler, such as with property or
        /// indexer calls.
        /// </summary>
        /// <param name="instance">Any instance.</param>
        public static void IgnoreValue(this object instance) { }
    }
}
