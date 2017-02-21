using System;
using System.Collections.Concurrent;

namespace Kingo.DynamicMethods
{
    /// <summary>
    /// When implemented, represents a method that is generated and compiled at runtime. The implementation of every
    /// dynamic method is based on the fields and properties of a certain instance's type.
    /// </summary>
    /// <typeparam name="TMethod">Type of the specific class implementing the method.</typeparam>
    public abstract class DynamicMethod<TMethod> where TMethod : DynamicMethod<TMethod>
    {
        #region [====== Instance Members ======]

        internal DynamicMethod() { }

        #endregion

        #region [====== Static Members ======]

        private static readonly ConcurrentDictionary<Type, TMethod> _Methods = new ConcurrentDictionary<Type, TMethod>();        

        internal static TMethod GetOrAddMethod(Type type, Func<Type, MemberProvider, TMethod> methodFactory)
        {
            return _Methods.GetOrAdd(type, t => methodFactory.Invoke(t, MemberProvider.For(t)));
        }

        #endregion
    }
}
