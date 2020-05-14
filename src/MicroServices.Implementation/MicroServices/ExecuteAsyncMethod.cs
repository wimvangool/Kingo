using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents the <see cref="IQuery{TResponse}.ExecuteAsync"/> or
    /// <see cref="IQuery{TRequest, TResponse}.ExecuteAsync"/> method of a specific query.
    /// </summary>
    public class ExecuteAsyncMethod : IAsyncMethod
    {
        internal ExecuteAsyncMethod(QueryComponent query) :
            this(query, query.Interfaces.Single()) { }

        internal ExecuteAsyncMethod(QueryComponent query, QueryInterface @interface) :
            this(query, @interface.ResolveMethodInfo(query)) { }

        private ExecuteAsyncMethod(QueryComponent query, MethodInfo info) :
            this(query, info, info.GetParameters()) { }

        private ExecuteAsyncMethod(QueryComponent query, MethodInfo info, ParameterInfo[] parameters)
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

        /// <inheritdoc />
        public override string ToString() =>
            $"{Query.Type.FriendlyName()}.{MethodInfo.Name}({string.Join(", ", Parameters())})";

        private IEnumerable<string> Parameters()
        {
            if (MessageParameterInfo != null)
            {
                yield return MessageParameterInfo.ParameterType.FriendlyName();
            }
            yield return "...";
        }

        #region [====== IAsyncMethod ======]

        Type IAsyncMethod.ComponentType =>
            Query.Type;        

        /// <summary>
        /// The query that implements this method.
        /// </summary>
        public QueryComponent Query
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
