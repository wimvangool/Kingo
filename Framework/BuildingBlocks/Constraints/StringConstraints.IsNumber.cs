using System;
using System.Globalization;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {                                                
        #region [====== IsByte ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse a byte.
        /// </summary>
        public const NumberStyles DefaultByteNumberStyles = NumberStyles.Integer;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="byte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, byte> IsByte<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsByte(member, DefaultByteNumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="byte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, byte> IsByte<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return member.Apply(new StringIsByteConstraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion

        #region [====== IsSByte ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse a sbyte.
        /// </summary>
        public const NumberStyles DefaultSByteNumberStyles = NumberStyles.Integer | NumberStyles.AllowTrailingSign;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, sbyte> IsSByte<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsSByte(member, DefaultSByteNumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, sbyte> IsSByte<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsSByteConstraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion

        #region [====== IsChar ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="char"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The only character of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, char> IsChar<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsCharConstraint().WithErrorMessage(errorMessage));
        }        

        #endregion

        #region [====== IsInt16 ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse a short.
        /// </summary>
        public const NumberStyles DefaultInt16NumberStyles = NumberStyles.Integer;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="short"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="short"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, short> IsInt16<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsInt16(member, DefaultInt16NumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="short"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="short"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, short> IsInt16<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsInt16Constraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion        

        #region [====== IsInt32 ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse an int.
        /// </summary>
        public const NumberStyles DefaultInt32NumberStyles = NumberStyles.Integer;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="int"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="int"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, int> IsInt32<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsInt32(member, DefaultInt32NumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="int"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="int"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, int> IsInt32<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsInt32Constraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion        

        #region [====== IsInt64 ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse a long.
        /// </summary>
        public const NumberStyles DefaultInt64NumberStyles = NumberStyles.Integer;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="long"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="long"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, long> IsInt64<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsInt64(member, DefaultInt64NumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="long"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="long"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, long> IsInt64<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsInt64Constraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion      

        #region [====== IsSingle ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse a float.
        /// </summary>
        public const NumberStyles DefaultSingleNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="float"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="float"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, float> IsSingle<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsSingle(member, DefaultSingleNumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="float"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="float"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, float> IsSingle<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsSingleConstraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion

        #region [====== IsDouble ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse a float.
        /// </summary>
        public const NumberStyles DefaultDoubleNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="double"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="double"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, double> IsDouble<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsDouble(member, DefaultDoubleNumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="double"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="double"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, double> IsDouble<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsDoubleConstraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion

        #region [====== IsDecimal ======]

        /// <summary>
        /// Defines the default <see cref="NumberStyles" /> value to parse a float.
        /// </summary>
        public const NumberStyles DefaultDecimalNumberStyles = NumberStyles.Float | NumberStyles.AllowThousands;

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="decimal"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="decimal"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, decimal> IsDecimal<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            return IsDecimal(member, DefaultDecimalNumberStyles, null, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="decimal"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="decimal"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, decimal> IsDecimal<TMessage>(this IMemberConstraint<TMessage, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsDecimalConstraint(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion        
    }

    #region [====== StringIsNumberConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a specific number type.
    /// </summary>
    public abstract class StringIsNumberConstraint<TValueOut> : Constraint<string, TValueOut>
    {
        private readonly NumberStyles _style;
        private readonly IFormatProvider _formatProvider;
    
        internal StringIsNumberConstraint(NumberStyles style, IFormatProvider formatProvider)
        {
            _style = style;
            _formatProvider = formatProvider;
        }

        internal StringIsNumberConstraint(StringIsNumberConstraint<TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _style = constraint._style;
            _formatProvider = constraint._formatProvider;
        }

        internal StringIsNumberConstraint(StringIsNumberConstraint<TValueOut> constraint, Identifier name)
            : base(constraint, name)
        {
            _style = constraint._style;
            _formatProvider = constraint._formatProvider;
        }

        /// <summary>
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in the value.
        /// </summary>
        public NumberStyles Style
        {
            get { return _style; }
        }

        /// <summary>
        /// An object that supplies culture-specific formatting information about the value.
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
        }

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string valueIn, out TValueOut valueOut)
        {
            return TryParse(valueIn, _style, _formatProvider, out valueOut);
        }
        
        internal abstract bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out TValueOut valueOut);

        #endregion
    }

    #endregion

    #region [====== StringIsByteConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a byte.
    /// </summary>
    public sealed class StringIsByteConstraint : StringIsNumberConstraint<byte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsByteConstraint" /> class.
        /// </summary>    
        public StringIsByteConstraint()
            : this(StringConstraints.DefaultByteNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsByteConstraint" /> class.
        /// </summary>    
        public StringIsByteConstraint(NumberStyles style, IFormatProvider formatProvider = null)
        : base(style, formatProvider) { }

        private StringIsByteConstraint(StringIsByteConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private StringIsByteConstraint(StringIsByteConstraint constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsByte); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, byte> WithName(Identifier name)
        {
            return new StringIsByteConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, byte> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsByteConstraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out byte valueOut)
        {
            return byte.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsSByteConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a sbyte.
    /// </summary>
    public sealed class StringIsSByteConstraint : StringIsNumberConstraint<sbyte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSByteConstraint" /> class.
        /// </summary>    
        public StringIsSByteConstraint()
            : this(StringConstraints.DefaultSByteNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSByteConstraint" /> class.
        /// </summary>    
        public StringIsSByteConstraint(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsSByteConstraint(StringIsSByteConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsSByteConstraint(StringIsSByteConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsSByte); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, sbyte> WithName(Identifier name)
        {
            return new StringIsSByteConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, sbyte> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsSByteConstraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out sbyte valueOut)
        {
            return sbyte.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsCharConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string contains only a single character.
    /// </summary>
    public sealed class StringIsCharConstraint : Constraint<string, char>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsCharConstraint" /> class.
        /// </summary>    
        public StringIsCharConstraint() {}

        private StringIsCharConstraint(StringIsCharConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private StringIsCharConstraint(StringIsCharConstraint constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsChar); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, char> WithName(Identifier name)
        {
            return new StringIsCharConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, char> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsCharConstraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string valueIn, out char valueOut)
        {
            if (valueIn == null)
            {
                throw new ArgumentNullException("valueIn");
            }
            if (valueIn.Length == 1)
            {
                valueOut = valueIn[0];
                return true;
            }
            valueOut = '\0';
            return false;
        }

        #endregion
    }

    #endregion

    #region [====== StringIsInt16Constraint ======]

    /// <summary>
    /// Represents a constrashort that checks whether or not a string can be converted to a short.
    /// </summary>
    public sealed class StringIsInt16Constraint : StringIsNumberConstraint<short>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt16Constraint" /> class.
        /// </summary>    
        public StringIsInt16Constraint()
            : this(StringConstraints.DefaultInt16NumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt16Constraint" /> class.
        /// </summary>    
        public StringIsInt16Constraint(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsInt16Constraint(StringIsInt16Constraint constrashort, StringTemplate errorMessage)
            : base(constrashort, errorMessage) { }

        private StringIsInt16Constraint(StringIsInt16Constraint constrashort, Identifier name)
            : base(constrashort, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsInt16); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, short> WithName(Identifier name)
        {
            return new StringIsInt16Constraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, short> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsInt16Constraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out short valueOut)
        {
            return short.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsInt32Constraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a int.
    /// </summary>
    public sealed class StringIsInt32Constraint : StringIsNumberConstraint<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt32Constraint" /> class.
        /// </summary>    
        public StringIsInt32Constraint()
            : this(StringConstraints.DefaultInt32NumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt32Constraint" /> class.
        /// </summary>    
        public StringIsInt32Constraint(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsInt32Constraint(StringIsInt32Constraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsInt32Constraint(StringIsInt32Constraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsInt32); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, int> WithName(Identifier name)
        {
            return new StringIsInt32Constraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, int> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsInt32Constraint(this, errorMessage);
        }

        #endregion       

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out int valueOut)
        {
            return int.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsInt64Constraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a long.
    /// </summary>
    public sealed class StringIsInt64Constraint : StringIsNumberConstraint<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt64Constraint" /> class.
        /// </summary>    
        public StringIsInt64Constraint()
            : this(StringConstraints.DefaultInt64NumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt64Constraint" /> class.
        /// </summary>    
        public StringIsInt64Constraint(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsInt64Constraint(StringIsInt64Constraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsInt64Constraint(StringIsInt64Constraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsInt64); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, long> WithName(Identifier name)
        {
            return new StringIsInt64Constraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, long> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsInt64Constraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out long valueOut)
        {
            return long.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsSingleConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a float.
    /// </summary>
    public sealed class StringIsSingleConstraint : StringIsNumberConstraint<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSingleConstraint" /> class.
        /// </summary>    
        public StringIsSingleConstraint()
            : this(StringConstraints.DefaultSingleNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSingleConstraint" /> class.
        /// </summary>    
        public StringIsSingleConstraint(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsSingleConstraint(StringIsSingleConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsSingleConstraint(StringIsSingleConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsSingle); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, float> WithName(Identifier name)
        {
            return new StringIsSingleConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, float> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsSingleConstraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out float valueOut)
        {
            return float.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsDoubleConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a double.
    /// </summary>
    public sealed class StringIsDoubleConstraint : StringIsNumberConstraint<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDoubleConstraint" /> class.
        /// </summary>    
        public StringIsDoubleConstraint()
            : this(StringConstraints.DefaultDoubleNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDoubleConstraint" /> class.
        /// </summary>    
        public StringIsDoubleConstraint(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsDoubleConstraint(StringIsDoubleConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsDoubleConstraint(StringIsDoubleConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsDouble); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, double> WithName(Identifier name)
        {
            return new StringIsDoubleConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, double> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsDoubleConstraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out double valueOut)
        {
            return double.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsDecimalConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string can be converted to a decimal.
    /// </summary>
    public sealed class StringIsDecimalConstraint : StringIsNumberConstraint<decimal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDecimalConstraint" /> class.
        /// </summary>    
        public StringIsDecimalConstraint()
            : this(StringConstraints.DefaultDecimalNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDecimalConstraint" /> class.
        /// </summary>    
        public StringIsDecimalConstraint(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsDecimalConstraint(StringIsDecimalConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsDecimalConstraint(StringIsDecimalConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsDecimal); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, decimal> WithName(Identifier name)
        {
            return new StringIsDecimalConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string, decimal> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsDecimalConstraint(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        internal override bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out decimal valueOut)
        {
            return decimal.TryParse(valueIn, style, formatProvider, out valueOut);
        }

        #endregion
    }

    #endregion

}
