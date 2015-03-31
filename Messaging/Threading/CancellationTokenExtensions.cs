namespace System.Threading
{
    /// <summary>
    /// Contains extension-methods for a nullable <see cref="CancellationToken" />.
    /// </summary>
    public static class CancellationTokenExtensions
    {
        /// <summary>
        /// Throws an <see cref="OperationCanceledException" /> if a token is specified and indicates
        /// that cancellation is requested.
        /// </summary>
        /// <param name="token">The token to check for a cancellation request.</param>
        public static void ThrowIfCancellationRequested(this CancellationToken? token)
        {
            if (token.HasValue)
            {
                token.Value.ThrowIfCancellationRequested();
            }
        }
    }
}
