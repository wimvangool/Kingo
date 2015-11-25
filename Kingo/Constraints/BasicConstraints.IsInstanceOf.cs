using System;
using Kingo.Resources;

namespace Kingo.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsNotInstanceOf ======]

        /// <summary>
        /// Verifies that this member's value is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraintBuilder<T, TValue> IsNotInstanceOf<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Type type, string errorMessage = null)
        {
            return member.Apply(NewIsNotInstanceOfConstraint<TValue>(type, errorMessage));
        }

        /// <summary>
        /// Verifies that this member's value is not an instance of <paramref name="typeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="typeFactory">Delegate that returns the type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraintBuilder<T, TValue> IsNotInstanceOf<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, Type> typeFactory, string errorMessage = null)
        {
            if (typeFactory == null)
            {
                throw new ArgumentNullException("typeFactory");
            }
            return member.Apply(message => NewIsNotInstanceOfConstraint<TValue>(typeFactory.Invoke(message), errorMessage));
        }

        /// <summary>
        /// Creates and returns a constraint that checks whether or not a certain value is of the specified <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="type">Type that the value is compared to.</param>
        /// <param name="errorMessage">Error message of the constraint.</param>
        /// <returns>A new constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static IConstraint<TValue> NewIsNotInstanceOfConstraint<TValue>(Type type, string errorMessage = null)
        {
            return NewIsInstanceOfConstraint<TValue>(type).Invert(errorMessage);
        }

        #endregion

        #region [====== IsInstanceOf ======]

        /// <summary>
        /// Verifies that the member's value is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="type">The type to compare the member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraintBuilder<T, TValue> IsInstanceOf<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Type type, string errorMessage = null)
        {
            return member.Apply(NewIsInstanceOfConstraint<TValue>(type).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is an instance of <paramref name="typeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="typeFactory">Delegate that returns the type to compare the member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraintBuilder<T, TValue> IsInstanceOf<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, Func<T, Type> typeFactory, string errorMessage = null)
        {
            if (typeFactory == null)
            {
                throw new ArgumentNullException("typeFactory");
            }
            return member.Apply(message => NewIsInstanceOfConstraint<TValue>(typeFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }        

        /// <summary>
        /// Creates and returns a constraint that checks whether or not a certain value is of the specified <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="type">Type that the value is compared to or cast to.</param>
        /// <param name="errorMessage">Error message of the constraint.</param>
        /// <returns>A new constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue> NewIsInstanceOfConstraint<TValue>(Type type, string errorMessage = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            var constraintTypeDefinition = typeof(IsInstanceOfFilter<,>);
            var constraintType = constraintTypeDefinition.MakeGenericType(typeof(TValue), type);
            return (IConstraintWithErrorMessage<TValue>) Activator.CreateInstance(constraintType);
        }

        #endregion
    }

    #region [====== IsInstanceOfFilter ======]

    /// <summary>
    /// Represents a filter that transforms a value into another type of value.
    /// </summary>
    public sealed class IsInstanceOfFilter<TValueIn, TValueOut> : Filter<TValueIn, TValueOut>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsInstanceOfFilter{T, S}" /> class.
        /// </summary>    
        public IsInstanceOfFilter() { }

        private IsInstanceOfFilter(IsInstanceOfFilter<TValueIn, TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private IsInstanceOfFilter(IsInstanceOfFilter<TValueIn, TValueOut> constraint, Identifier name)
            : base(constraint, name) { }

        /// <summary>
        /// Returns the type that the value is compared to or cast to.
        /// </summary>
        public Type Type
        {
            get { return typeof(TValueOut); }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsInstanceOf); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name)
        {
            return new IsInstanceOfFilter<TValueIn, TValueOut>(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsInstanceOfFilter<TValueIn, TValueOut>(this, errorMessage);
        }

        #endregion     
   
        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<TValueIn>(this, ErrorMessages.BasicConstraints_IsNotInstanceOf)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value)
        {
            return value is TValueOut;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value, out TValueOut valueOut)
        {
            // NB: A NullReferenceException is thrown when value is null
            // and TValueOut is a ValueType.
            object valueIn = value;

            try
            {
                valueOut = (TValueOut) valueIn;
                return true;
            }
            catch (NullReferenceException)
            {                
                valueOut = default(TValueOut);
                return false;
            }
            catch (InvalidCastException)
            {
                valueOut = default(TValueOut);
                return false;
            }
        }

        #endregion
    }

    #endregion

    #region [====== AsFilter ======]

    /// <summary>
    /// Represents a filter that transforms a value into another type of value. This filter
    /// always succeeds, but outputs a <c>null</c> value if the conversion failed.
    /// </summary>
    public sealed class AsFilter<TValueIn, TValueOut> : Filter<TValueIn, TValueOut> where TValueOut : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsFilter{T, S}" /> class.
        /// </summary>    
        public AsFilter() { }

        private AsFilter(AsFilter<TValueIn, TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private AsFilter(AsFilter<TValueIn, TValueOut> constraint, Identifier name)
            : base(constraint, name) { }        

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name)
        {
            return new AsFilter<TValueIn, TValueOut>(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage)
        {
            return new AsFilter<TValueIn, TValueOut>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<TValueIn>(this, ErrorMessages.BasicConstraints_IsNotInstanceOf)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value)
        {
            return value is TValueOut;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value, out TValueOut valueOut)
        {
            valueOut = value as TValueOut;
            return true;
        }

        #endregion
    }

    #endregion
}
