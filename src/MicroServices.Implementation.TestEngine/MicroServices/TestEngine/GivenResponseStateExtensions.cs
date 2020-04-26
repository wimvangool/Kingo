using System;
using System.Threading.Tasks;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IGivenResponseState{TResponse" /> and <see cref="IGivenResponseState{TRequest, TResponse}"/>.
    /// </summary>
    public static class GivenResponseStateExtensions
    {
        #region [====== IsExecutedByQuery (1) ======]

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TResponse>(this IGivenResponseState<TResponse> state, Func<IQueryOperationContext, TResponse> query, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator = null) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TResponse>.Decorate(query), configurator);

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TResponse>(this IGivenResponseState<TResponse> state, Func<IQueryOperationContext, Task<TResponse>> query, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator = null) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TResponse>.Decorate(query), configurator);

        #endregion

        #region [====== IsExecutedByQuery (2) ======]

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="request">Request to execute by the query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TRequest, TResponse>(this IGivenResponseState<TRequest, TResponse> state, Func<TRequest, IQueryOperationContext, TResponse> query, TRequest request) =>
            state.IsExecutedBy(query, ConfigureRequest(request));

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="query"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TRequest, TResponse>(this IGivenResponseState<TRequest, TResponse> state, Func<TRequest, IQueryOperationContext, TResponse> query, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TRequest, TResponse>.Decorate(query), configurator);

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="request">Request to execute by the query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TRequest, TResponse>(this IGivenResponseState<TRequest, TResponse> state, Func<TRequest, IQueryOperationContext, Task<TResponse>> query, TRequest request) =>
            state.IsExecutedBy(query, ConfigureRequest(request));

        /// <summary>
        /// Prepares the request to be executed by the specified <paramref name="query" />.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="query">The query that will execute the request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/>, <paramref name="query"/> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        public static void IsExecutedBy<TRequest, TResponse>(this IGivenResponseState<TRequest, TResponse> state, Func<TRequest, IQueryOperationContext, Task<TResponse>> query, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) =>
            NotNull(state).IsExecutedBy(QueryDecorator<TRequest, TResponse>.Decorate(query), configurator);

        #endregion

        private static TState NotNull<TState>(TState state) where TState : class =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
