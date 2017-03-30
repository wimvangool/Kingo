using System;
using System.Globalization;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{String}" />.
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, byte> IsByte<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, byte> IsByte<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return member.Apply(new StringIsByteFilter(style, formatProvider).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, sbyte> IsSByte<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, sbyte> IsSByte<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsSByteFilter(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion

        #region [====== IsChar ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="char"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The only character of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, char> IsChar<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsCharFilter().WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="short"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, short> IsInt16<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="short"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, short> IsInt16<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsInt16Filter(style, formatProvider).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="int"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, int> IsInt32<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="int"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, int> IsInt32<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsInt32Filter(style, formatProvider).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="long"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, long> IsInt64<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="long"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, long> IsInt64<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsInt64Filter(style, formatProvider).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="float"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, float> IsSingle<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="float"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, float> IsSingle<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsSingleFilter(style, formatProvider).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="double"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, double> IsDouble<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="double"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, double> IsDouble<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsDoubleFilter(style, formatProvider).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="decimal"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, decimal> IsDecimal<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
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
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="decimal"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, decimal> IsDecimal<T>(this IMemberConstraintBuilder<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            return member.Apply(new StringIsDecimalFilter(style, formatProvider).WithErrorMessage(errorMessage));
        }        

        #endregion        
    }

    #region [====== StringIsNumberFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a number.
    /// </summary>
    public abstract class StringIsNumberFilter<TValueOut> : Filter<string, TValueOut>
    {
        /// <summary>
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in the value.
        /// </summary>
        public readonly NumberStyles Style;

        /// <summary>
        /// An object that supplies culture-specific formatting information about the value.
        /// </summary>
        public readonly IFormatProvider FormatProvider;
    
        internal StringIsNumberFilter(NumberStyles style, IFormatProvider formatProvider)
        {
            Style = style;
            FormatProvider = formatProvider;
        }

        internal StringIsNumberFilter(StringIsNumberFilter<TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Style = constraint.Style;
            FormatProvider = constraint.FormatProvider;
        }

        internal StringIsNumberFilter(StringIsNumberFilter<TValueOut> constraint, Identifier name)
            : base(constraint, name)
        {
            Style = constraint.Style;
            FormatProvider = constraint.FormatProvider;
        }        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string valueIn, out TValueOut valueOut)
        {
            return TryParse(valueIn, Style, FormatProvider, out valueOut);
        }
        
        internal abstract bool TryParse(string valueIn, NumberStyles style, IFormatProvider formatProvider, out TValueOut valueOut);

        #endregion
    }

    #endregion

    #region [====== StringIsByteFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a byte.
    /// </summary>
    public sealed class StringIsByteFilter : StringIsNumberFilter<byte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsByteFilter" /> class.
        /// </summary>    
        public StringIsByteFilter()
            : this(StringConstraints.DefaultByteNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsByteFilter" /> class.
        /// </summary>    
        public StringIsByteFilter(NumberStyles style, IFormatProvider formatProvider = null)
        : base(style, formatProvider) { }

        private StringIsByteFilter(StringIsByteFilter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private StringIsByteFilter(StringIsByteFilter constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsByte); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, byte> WithName(Identifier name)
        {
            return new StringIsByteFilter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, byte> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsByteFilter(this, errorMessage);
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

    #region [====== StringIsSByteFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into an sbyte.
    /// </summary>
    public sealed class StringIsSByteFilter : StringIsNumberFilter<sbyte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSByteFilter" /> class.
        /// </summary>    
        public StringIsSByteFilter()
            : this(StringConstraints.DefaultSByteNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSByteFilter" /> class.
        /// </summary>    
        public StringIsSByteFilter(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsSByteFilter(StringIsSByteFilter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsSByteFilter(StringIsSByteFilter constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsSByte); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, sbyte> WithName(Identifier name)
        {
            return new StringIsSByteFilter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, sbyte> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsSByteFilter(this, errorMessage);
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

    #region [====== StringIsCharFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a character.
    /// </summary>
    public sealed class StringIsCharFilter : Filter<string, char>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsCharFilter" /> class.
        /// </summary>    
        public StringIsCharFilter() {}

        private StringIsCharFilter(StringIsCharFilter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private StringIsCharFilter(StringIsCharFilter constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsChar); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, char> WithName(Identifier name)
        {
            return new StringIsCharFilter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, char> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsCharFilter(this, errorMessage);
        }

        #endregion        

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string valueIn, out char valueOut)
        {
            if (valueIn == null)
            {
                throw new ArgumentNullException(nameof(valueIn));
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

    #region [====== StringIsInt16Filter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a short.
    /// </summary>
    public sealed class StringIsInt16Filter : StringIsNumberFilter<short>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt16Filter" /> class.
        /// </summary>    
        public StringIsInt16Filter()
            : this(StringConstraints.DefaultInt16NumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt16Filter" /> class.
        /// </summary>    
        public StringIsInt16Filter(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsInt16Filter(StringIsInt16Filter constrashort, StringTemplate errorMessage)
            : base(constrashort, errorMessage) { }

        private StringIsInt16Filter(StringIsInt16Filter constrashort, Identifier name)
            : base(constrashort, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsInt16); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, short> WithName(Identifier name)
        {
            return new StringIsInt16Filter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, short> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsInt16Filter(this, errorMessage);
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

    #region [====== StringIsInt32Filter ======]

    /// <summary>
    /// Represents a filter that transforms a string into an int.
    /// </summary>
    public sealed class StringIsInt32Filter : StringIsNumberFilter<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt32Filter" /> class.
        /// </summary>    
        public StringIsInt32Filter()
            : this(StringConstraints.DefaultInt32NumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt32Filter" /> class.
        /// </summary>    
        public StringIsInt32Filter(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsInt32Filter(StringIsInt32Filter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsInt32Filter(StringIsInt32Filter constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsInt32); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, int> WithName(Identifier name)
        {
            return new StringIsInt32Filter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, int> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsInt32Filter(this, errorMessage);
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

    #region [====== StringIsInt64Filter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a long.
    /// </summary>
    public sealed class StringIsInt64Filter : StringIsNumberFilter<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt64Filter" /> class.
        /// </summary>    
        public StringIsInt64Filter()
            : this(StringConstraints.DefaultInt64NumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsInt64Filter" /> class.
        /// </summary>    
        public StringIsInt64Filter(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsInt64Filter(StringIsInt64Filter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsInt64Filter(StringIsInt64Filter constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsInt64); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, long> WithName(Identifier name)
        {
            return new StringIsInt64Filter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, long> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsInt64Filter(this, errorMessage);
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

    #region [====== StringIsSingleFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a float.
    /// </summary>
    public sealed class StringIsSingleFilter : StringIsNumberFilter<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSingleFilter" /> class.
        /// </summary>    
        public StringIsSingleFilter()
            : this(StringConstraints.DefaultSingleNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsSingleFilter" /> class.
        /// </summary>    
        public StringIsSingleFilter(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsSingleFilter(StringIsSingleFilter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsSingleFilter(StringIsSingleFilter constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsSingle); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, float> WithName(Identifier name)
        {
            return new StringIsSingleFilter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, float> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsSingleFilter(this, errorMessage);
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

    #region [====== StringIsDoubleFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a double.
    /// </summary>
    public sealed class StringIsDoubleFilter : StringIsNumberFilter<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDoubleFilter" /> class.
        /// </summary>    
        public StringIsDoubleFilter()
            : this(StringConstraints.DefaultDoubleNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDoubleFilter" /> class.
        /// </summary>    
        public StringIsDoubleFilter(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsDoubleFilter(StringIsDoubleFilter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsDoubleFilter(StringIsDoubleFilter constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsDouble); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, double> WithName(Identifier name)
        {
            return new StringIsDoubleFilter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, double> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsDoubleFilter(this, errorMessage);
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

    #region [====== StringIsDecimalFilter ======]

    /// <summary>
    /// Represents a filter that transforms a string into a decimal.
    /// </summary>
    public sealed class StringIsDecimalFilter : StringIsNumberFilter<decimal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDecimalFilter" /> class.
        /// </summary>    
        public StringIsDecimalFilter()
            : this(StringConstraints.DefaultDecimalNumberStyles) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsDecimalFilter" /> class.
        /// </summary>    
        public StringIsDecimalFilter(NumberStyles style, IFormatProvider formatProvider = null)
            : base(style, formatProvider) { }

        private StringIsDecimalFilter(StringIsDecimalFilter constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsDecimalFilter(StringIsDecimalFilter constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_IsDecimal); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, decimal> WithName(Identifier name)
        {
            return new StringIsDecimalFilter(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<string, decimal> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsDecimalFilter(this, errorMessage);
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
