using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
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
        public static IMemberConstraint<T, TValue> IsNotInstanceOf<T, TValue>(this IMemberConstraint<T, TValue> member, Type type, string errorMessage = null)
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
        public static IMemberConstraint<T, TValue> IsNotInstanceOf<T, TValue>(this IMemberConstraint<T, TValue> member, Func<T, Type> typeFactory, string errorMessage = null)
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
        public static IMemberConstraint<T, TValue> IsInstanceOf<T, TValue>(this IMemberConstraint<T, TValue> member, Type type, string errorMessage = null)
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
        public static IMemberConstraint<T, TValue> IsInstanceOf<T, TValue>(this IMemberConstraint<T, TValue> member, Func<T, Type> typeFactory, string errorMessage = null)
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
            var constraintTypeDefinition = typeof(IsInstanceOfConstraint<,>);
            var constraintType = constraintTypeDefinition.MakeGenericType(typeof(TValue), type);
            return (IConstraintWithErrorMessage<TValue>) Activator.CreateInstance(constraintType);
        }

        #endregion
    }

    #region [====== IsInstanceOfConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is an instance of a specific type.
    /// </summary>
    public sealed class IsInstanceOfConstraint<TValueIn, TValueOut> : Constraint<TValueIn, TValueOut>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsInstanceOfConstraint{T, S}" /> class.
        /// </summary>    
        public IsInstanceOfConstraint() { }

        private IsInstanceOfConstraint(IsInstanceOfConstraint<TValueIn, TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private IsInstanceOfConstraint(IsInstanceOfConstraint<TValueIn, TValueOut> constraint, Identifier name)
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
        public override IConstraintWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name)
        {
            return new IsInstanceOfConstraint<TValueIn, TValueOut>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsInstanceOfConstraint<TValueIn, TValueOut>(this, errorMessage);
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

    #region [====== AsConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is an instance of a specific type. This constraints
    /// always succeeds, but outputs a <c>null</c> value if the conversion failed.
    /// </summary>
    public sealed class AsConstraint<TValueIn, TValueOut> : Constraint<TValueIn, TValueOut> where TValueOut : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsConstraint{T, S}" /> class.
        /// </summary>    
        public AsConstraint() { }

        private AsConstraint(AsConstraint<TValueIn, TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private AsConstraint(AsConstraint<TValueIn, TValueOut> constraint, Identifier name)
            : base(constraint, name) { }        

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name)
        {
            return new AsConstraint<TValueIn, TValueOut>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage)
        {
            return new AsConstraint<TValueIn, TValueOut>(this, errorMessage);
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
