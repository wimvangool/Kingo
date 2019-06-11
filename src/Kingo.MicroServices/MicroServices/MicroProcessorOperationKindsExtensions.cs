namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for <see cref="MicroProcessorOperationKinds" />.
    /// </summary>
    public static class MicroProcessorOperationKindsExtensions
    {
        /// <summary>
        /// Determines whether or not the specified <paramref name="operationKind"/> is supported based on the specified
        /// <paramref name="supportedOperationKinds"/>.
        /// </summary>
        /// <param name="operationKind">The operation type to check.</param>
        /// <param name="supportedOperationKinds">The supported operation types.</param>
        /// <returns><c>true</c> if <paramref name="operationKind"/> is supported; otherwise <c>false</c>.</returns>
        public static bool IsSupportedBy(this MicroProcessorOperationKinds operationKind, MicroProcessorOperationKinds supportedOperationKinds) =>
            supportedOperationKinds.HasFlag(operationKind);


    }
}
