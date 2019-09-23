using System;
using System.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents the <see cref="IQuery{TResponse}.ExecuteAsync"/> or
    /// <see cref="IQuery{TRequest, TResponse}.ExecuteAsync"/> method of a specific query.
    /// </summary>
    public class ExecuteAsyncMethod : IAsyncMethod
    {
        internal ExecuteAsyncMethod(Query query, QueryInterface @interface) :
            this(query, @interface.ResolveMethodInfo(query)) { }

        private ExecuteAsyncMethod(Query query, MethodInfo info) :
            this(query, info, info.GetParameters()) { }

        private ExecuteAsyncMethod(Query query, MethodInfo info, ParameterInfo[] parameters)
        {
            Query = query;
            MethodInfo = info;

            if (parameters.Length == 1)
            {
                MessageParameterInfo = null;
                ContextParameterInfo = parameters[0];
            }
            else
            {
                MessageParameterInfo = parameters[0];
                ContextParameterInfo = parameters[1];
            }
        }

        #region [====== IAsyncMethod ======]

        Type IAsyncMethod.ComponentType =>
            Query.Type;        

        /// <summary>
        /// The query that implements this method.
        /// </summary>
        public Query Query
        {
            get;
        }

        /// <inheritdoc />
        public MethodInfo MethodInfo
        {
            get;
        }

        /// <inheritdoc />
        public ParameterInfo MessageParameterInfo
        {
            get;
        }

        /// <inheritdoc />
        public ParameterInfo ContextParameterInfo
        {
            get;
        }

        #endregion
    }
}
