using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// When implemented by a class, represents an operand that can be verified in a test.
    /// </summary>
    /// <typeparam name="T">Type of operand's value.</typeparam>
    /// <remarks>
    /// <para>
    /// This interface provides a number of methods to perform some basic verifications of the operand's value. The
    /// interface also easily allows extenion-methods to be defined to handle more advanced verification on operand's
    /// of specified types.
    /// </para>
    /// <para>
    /// When verification methods are implemented, they should always have a <c>void</c> return type. If a verification
    /// fails, one of the <see cref="IOperand{T}.Scenario">scenario's</see> fail-methods should be called with a
    /// descriptive message. The implementor of the scenario can then determine how to handle any verification
    /// failures, such as by throwing an exception.
    /// </para>
    /// </remarks>
    public interface IOperand<T>
    {
        /// <summary>
        /// The scenario for which the verification takes place.
        /// </summary>
        IScenario Scenario
        {
            get;
        }

        /// <summary>
        /// Returns the value of the operand.
        /// </summary>
        T Value
        {
            get;
        }

        /// <summary>
        /// Verifies that the value of the operand is <c>null</c>.
        /// </summary>
        void IsNull();

        /// <summary>
        /// Verifies that the value of the operand is not <c>null</c>.
        /// </summary>
        void IsNotNull();

        /// <summary>
        /// Verifies that the operand's value is runtime compatible with the specified type.
        /// </summary>
        /// <typeparam name="TExpected">The expected type.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004", Justification = "Type parameter was added deliberately for ease of use.")]
        IConjunctionStatement<TExpected> IsA<TExpected>();

        /// <summary>
        /// Verifies that the operand's value is runtime compatible with the specified type.
        /// </summary>
        /// <param name="type">The expected type.</param>
        void IsA(Type type);

        /// <summary>
        /// Verifies that the operand's value is not runtime compatible with the specified type.
        /// </summary>
        /// <typeparam name="TUnexpected">The unexpected type.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004", Justification = "Type parameter was added deliberately for ease of use.")]
        void IsNotA<TUnexpected>();

        /// <summary>
        /// Verifies that the operand's value is not runtime compatible with the specified type.
        /// </summary>    
        void IsNotA(Type type);

        /// <summary>
        /// Verifies that the operand's value is the same instance as the specified value.
        /// </summary>
        /// <param name="expression">Another value.</param>
        void IsSameInstanceAs(T expression);

        /// <summary>
        /// Verifies that the operand's value is not the same instance as the specified value.
        /// </summary>
        /// <param name="expression">Another value.</param>
        void IsNotSameInstanceAs(T expression);

        /// <summary>
        /// Verifies that the operand's value is equal to the specified value.
        /// </summary>
        /// <param name="expression">Another value.</param>
        void IsEqualTo(T expression);

        /// <summary>
        /// Verifies that the operand's value is equal to the specified value as determined by the comparer.
        /// </summary>
        /// <param name="expression">Another value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <remarks>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer is used.
        /// </remarks>
        void IsEqualTo(T expression, IEqualityComparer<T> comparer);

        /// <summary>
        /// Verifies that the operand's value is not equal to the specified value.
        /// </summary>
        /// <param name="expression">Another value.</param>
        void IsNotEqualTo(T expression);

        /// <summary>
        /// Verifies that the operand's value is not equal to the specified value as determined by the comparer.
        /// </summary>
        /// <param name="expression">Another value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <remarks>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer is used.
        /// </remarks>
        void IsNotEqualTo(T expression, IEqualityComparer<T> comparer);

        /// <summary>
        /// Verifies that the operand's value is smaller than the specified value as determined by the comparer.
        /// </summary>
        /// <param name="expression">Another value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <remarks>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer is used.
        /// </remarks>
        void IsSmallerThan(T expression, IComparer<T> comparer);

        /// <summary>
        /// Verifies that the operand's value is smaller than or equal to the specified value as determined by the comparer.
        /// </summary>
        /// <param name="expression">Another value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <remarks>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer is used.
        /// </remarks>
        void IsSmallerThanOrEqualTo(T expression, IComparer<T> comparer);

        /// <summary>
        /// Verifies that the operand's value is greater than the specified value as determined by the comparer.
        /// </summary>
        /// <param name="expression">Another value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <remarks>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer is used.
        /// </remarks>
        void IsGreaterThan(T expression, IComparer<T> comparer);

        /// <summary>
        /// Verifies that the operand's value is greater than or equal to the specified value as determined by the comparer.
        /// </summary>
        /// <param name="expression">Another value.</param>
        /// <param name="comparer">Comparer to use.</param>
        /// <remarks>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer is used.
        /// </remarks>
        void IsGreaterThanOrEqualTo(T expression, IComparer<T> comparer);
    }
}
