using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IWhenCommandOrEventState{TMessage}" />.
    /// </summary>
    public static class WhenReturningStateExtensions
    {
        #region [====== IsExecutedByQuery (1) ======]

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TResponse> IsExecutedByQuery<TResponse>(this IWhenReturningState<TResponse> state, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator, Func<IQueryOperationContext, TResponse> query) =>
            NotNull(state).IsExecutedByQuery(configurator, QueryDecorator<TResponse>.Decorate(query));

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TResponse> IsExecutedByQuery<TResponse>(this IWhenReturningState<TResponse> state, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator, Func<IQueryOperationContext, Task<TResponse>> query) =>
            NotNull(state).IsExecutedByQuery(configurator, QueryDecorator<TResponse>.Decorate(query));

        /// <summary>
        /// Schedules the (void) request to be executed by the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">Query that will execute the (void) request.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TResponse> IsExecutedByQuery<TResponse>(this IWhenReturningState<TResponse> state, IQuery<TResponse> query) =>
            NotNull(state).IsExecutedByQuery(null, query);

        #endregion

        #region [====== IsExecutedByQuery (2) ======]

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedByQuery<TRequest, TResponse>(this IWhenReturningState<TRequest, TResponse> state, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator, Func<TRequest, IQueryOperationContext, TResponse> query) =>
            NotNull(state).IsExecutedByQuery(configurator, QueryDecorator<TRequest, TResponse>.Decorate(query));

        /// <summary>
        /// Prepares the command to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="configurator"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedByQuery<TRequest, TResponse>(this IWhenReturningState<TRequest, TResponse> state, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator, Func<TRequest, IQueryOperationContext, Task<TResponse>> query) =>
            NotNull(state).IsExecutedByQuery(configurator, QueryDecorator<TRequest, TResponse>.Decorate(query));

        #endregion

        private static TState NotNull<TState>(TState state) where TState : class =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
