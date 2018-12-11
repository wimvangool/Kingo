namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Contains extensions methods for enum <see cref="AggregateSerializationStrategy" />.
    /// </summary>
    public static class AggregateSerializationStrategyExtensions
    {
        /// <summary>
        /// Indicates whether or not the specified <paramref name="strategy" /> uses snapshots.
        /// </summary>
        /// <param name="strategy">The strategy to check.</param>
        /// <returns><c>true</c> if the strategy uses snapshots; otherwise <c>false</c>.</returns>
        public static bool UsesSnapshots(this AggregateSerializationStrategy strategy) =>
            strategy.HasFlag(AggregateSerializationStrategy.Snapshots);

        /// <summary>
        /// Indicates whether or not the specified <paramref name="strategy" /> uses events.
        /// </summary>
        /// <param name="strategy">The strategy to check.</param>
        /// <returns><c>true</c> if the strategy uses events; otherwise <c>false</c>.</returns>
        public static bool UsesEvents(this AggregateSerializationStrategy strategy) =>
            strategy.HasFlag(AggregateSerializationStrategy.Events);
    }
}
