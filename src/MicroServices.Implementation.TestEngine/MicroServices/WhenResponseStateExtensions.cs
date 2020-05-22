using System;
using System.Threading.Tasks;
using static Kingo.MicroServices.MicroProcessorTestContext;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IWhenCommandState{TMessage}" />.
    /// </summary>
    public static class WhenResponseStateExtensions
    {
        #region [====== IsExecutedByQuery (1) ======]

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TResponse> IsExecutedBy<TResponse>(this IWhenResponseState<TResponse> state, Func<QueryOperationContext, TResponse> query, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator = null) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TResponse>.Decorate(query), configurator);

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TResponse> IsExecutedBy<TResponse>(this IWhenResponseState<TResponse> state, Func<QueryOperationContext, Task<TResponse>> query, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator = null) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TResponse>.Decorate(query), configurator);

        #endregion

        #region [====== IsExecutedByQuery (2) ======]

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="request">Request to execute by the query.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy<TRequest, TResponse>(this IWhenResponseState<TRequest, TResponse> state, Func<TRequest, QueryOperationContext, TResponse> query, TRequest request) =>
            state.IsExecutedBy(query, ConfigureRequest(request));

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="query"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy<TRequest, TResponse>(this IWhenResponseState<TRequest, TResponse> state, Func<TRequest, QueryOperationContext, TResponse> query, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TRequest, TResponse>.Decorate(query), configurator);

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="request">Request to execute by the query.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy<TRequest, TResponse>(this IWhenResponseState<TRequest, TResponse> state, Func<TRequest, QueryOperationContext, Task<TResponse>> query, TRequest request) =>
            state.IsExecutedBy(query, ConfigureRequest(request));

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="query"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy<TRequest, TResponse>(this IWhenResponseState<TRequest, TResponse> state, Func<TRequest, QueryOperationContext, Task<TResponse>> query, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TRequest, TResponse>.Decorate(query), configurator);

        #endregion

        private static TState NotNull<TState>(TState state) where TState : class =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
