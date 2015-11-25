using System;
using System.Linq;
using Kingo.Resources;

namespace Kingo.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T, TValue}" />.
    /// </summary>
    public static class EnumConstraints
    {
        #region [====== IsInRangeOfValidValues ======]

        /// <summary>
        /// Verifies whether or not the specified <paramref name="member"/> does not contain any value or bitflags
        /// that is/are not in range of acceptable values of the defining Enum type.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraintBuilder<T, TValue> IsInRangeOfValidValues<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, string errorMessage = null)
            where TValue : struct
        {
            return member.Apply(new EnumIsInRangeOfValidValuesConstraint<TValue>().WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> does not contain any value or bitflags
        /// that is/are not in range of acceptable values of the defining Enum type.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is not a Flags enum and represents one of the allowed values, or
        /// if <paramref name="value"/> is a flags enum and all flags are valid flags of this enum;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <typeparamref name="TEnum"/> is not an Enum type.
        /// </exception>
        public static bool IsInRangeOfValidValues<TEnum>(TEnum value) where TEnum : struct
        {
            var enumFlags = CastToEnum(value);
            if (enumFlags.IsFlagsEnum())
            {
                return CastToEnum(EnumOperators<TEnum>.AllValuesCombined()).HasFlag(enumFlags);
            }
            return enumFlags.IsDefined();            
        }              

        /// <summary>
        /// Determines whether or not the enum is decorated with the <see cref="FlagsAttribute" />.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="FlagsAttribute" /> has been declared on the specified <paramref name="value"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool IsFlagsEnum(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Any();
        }

        #endregion

        #region [====== IsDefined ======]

        /// <summary>
        /// Verifies whether or not the specified <paramref name="member"/> is defined as a constant by the specified Enum type.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraintBuilder<T, TValue> IsDefined<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, string errorMessage = null)
            where TValue : struct
        {
            return member.Apply(new EnumIsDefinedConstraint<TValue>().WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> is valid relative to the values that have been
        /// declare on the enum type itself.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is not a Flags enum and represents one of the allowed values, or
        /// if <paramref name="value"/> is a flags enum and all flags are valid flags of this enum;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public static bool IsDefined(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return Enum.IsDefined(value.GetType(), value);
        }

        #endregion

        #region [====== HasFlag ======]

        /// <summary>
        /// Verifies whether or not the specified <paramref name="member"/> has all specified flags of the <paramref name="flag"/> value set.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="flag">The flag(s) to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="flag"/> is not an Enum type or <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraintBuilder<T, TValue> HasFlag<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, TValue flag, string errorMessage = null)
            where TValue : struct
        {
            return member.Apply(new EnumHasFlagConstraint<TValue>(flag).WithErrorMessage(errorMessage));
        }

        #endregion

        internal static Enum CastToEnum(object value)
        {
            var enumValue = value as Enum;
            if (enumValue == null)
            {
                throw NewUnsupportedValueException(value);
            }
            return enumValue;
        }

        private static Exception NewUnsupportedValueException(object value)
        {
            var messageFormat = ExceptionMessages.EnumIsInRangeOfValidValues_UnsupportedValue;
            var message = string.Format(messageFormat, value, value.GetType());
            return new ArgumentException(message, "value");
        }  
    }

    #region [====== EnumIsInRangeOfValidValuesConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not an Enum value has any value or bitflags
    /// that is/are not in range of acceptable values of the defining Enum type.
    /// </summary>
    public sealed class EnumIsInRangeOfValidValuesConstraint<TValue> : Constraint<TValue> where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumIsInRangeOfValidValuesConstraint{T}" /> class.
        /// </summary>    
        public EnumIsInRangeOfValidValuesConstraint() {}

        private EnumIsInRangeOfValidValuesConstraint(EnumIsInRangeOfValidValuesConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private EnumIsInRangeOfValidValuesConstraint(EnumIsInRangeOfValidValuesConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) {}

        /// <summary>
        /// Returns the type of enumeration this constraint is for.
        /// </summary>
        public Type EnumType
        {
            get { return typeof(TValue); }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.EnumConstraints_IsInRangeOfValidValues); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new EnumIsInRangeOfValidValuesConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new EnumIsInRangeOfValidValuesConstraint<TValue>(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return EnumConstraints.IsInRangeOfValidValues(value);
        }

        #endregion
    }

    #endregion

    #region [====== EnumIsDefinedConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not an enum value is within range of its valid values.
    /// </summary>
    public sealed class EnumIsDefinedConstraint<TValue> : Constraint<TValue> where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumIsDefinedConstraint{T}" /> class.
        /// </summary>    
        public EnumIsDefinedConstraint() {}

        private EnumIsDefinedConstraint(EnumIsDefinedConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private EnumIsDefinedConstraint(EnumIsDefinedConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) {}

        /// <summary>
        /// Returns the type of enumeration this constraint is for.
        /// </summary>
        public Type EnumType
        {
            get { return typeof(TValue); }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.EnumConstraints_IsDefined); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new EnumIsDefinedConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new EnumIsDefinedConstraint<TValue>(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {                        
            return EnumConstraints.CastToEnum(value).IsDefined();
        }
              
        #endregion
    }

    #endregion

    #region [====== EnumHasFlagConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not all bitflags of a specific Enum value are set on the specified (checked) value.
    /// </summary>
    public sealed class EnumHasFlagConstraint<TValue> : Constraint<TValue>
        where TValue : struct
    {
        /// <summary>
        /// The flag(s) to check.
        /// </summary>
        public readonly Enum Flag;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumHasFlagConstraint{T}" /> class.
        /// </summary>    
        /// <param name="flag">The flag(s) to check.</param>
        public EnumHasFlagConstraint(TValue flag)
        {
            Flag = EnumConstraints.CastToEnum(flag);
        }

        private EnumHasFlagConstraint(EnumHasFlagConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Flag = constraint.Flag;
        }

        private EnumHasFlagConstraint(EnumHasFlagConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            Flag = constraint.Flag;
        }        

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.EnumConstraints_HasFlag); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new EnumHasFlagConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new EnumHasFlagConstraint<TValue>(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return EnumConstraints.CastToEnum(value).HasFlag(Flag);
        }

        #endregion
    }

    #endregion
}
