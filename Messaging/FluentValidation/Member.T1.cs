using System.Collections.Generic;
using System.ComponentModel.Resources;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represent a certain member that can be validated and produces an <see cref="ErrorMessage" /> if this validation fails.
    /// </summary>
    /// <typeparam name="TValue">Type of the member's value.</typeparam>
    public sealed class Member<TValue> : IMember
    {
        private readonly MemberSet _memberSet;
        private readonly string _parentName;
        private readonly string _name;
        private readonly Lazy<TValue> _value;
        private Constraint _constraint;
        
        internal Member(MemberSet memberSet, Func<TValue> valueFactory, string name, string parentName = null)
            : this(memberSet, valueFactory, name, parentName, new NullConstraint()) { }

        private Member(MemberSet memberSet, Func<TValue> valueFactory, string name, string parentName, Constraint constraint)
        {
            _memberSet = memberSet;
            _value = new Lazy<TValue>(valueFactory);               
            _name = name;
            _parentName = parentName;         
            _constraint = constraint;
        }                               

        #region [====== IMember & IErrorMessageProducer ======]               

        /// <inheritdoc />
        public string FullName
        {
            get
            {
                if (_parentName == null)
                {
                    return _name;
                }
                return string.Format("{0}.{1}", _parentName, _name);
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
        }

        object IMember.Value
        {
            get { return Value; }
        }

        /// <summary>
        /// The value of this member.
        /// </summary>
        public TValue Value
        {
            get { return _value.Value; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return FullName;
        }

        void IErrorMessageProducer.AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {
            _constraint.AddErrorMessagesTo(consumer);
        }                

        #endregion

        #region [====== And ======]

        /// <summary>
        /// Descends one level down in the validation-hierarchy.
        /// </summary>
        /// <param name="innerConstraintFactory">
        /// The delegate that is used to define constraint on the properties or children of this member's value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerConstraintFactory"/> is <c>null</c>.
        /// </exception>
        public void And(Action<TValue> innerConstraintFactory)
        {
            if (innerConstraintFactory == null)
            {
                throw new ArgumentNullException("innerConstraintFactory");
            }
            _memberSet.PushParent(_name);

            try
            {
                innerConstraintFactory.Invoke(Value);
            }
            finally
            {
                _memberSet.PopParent();
            }
        }

        #endregion

        #region [====== IsNotNull ======]

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>        
        /// <returns>This member.</returns>        
        public Member<TValue> IsNotNull()
        {
            return Satisfies(IsNotNull, new ErrorMessage(ValidationMessages.Member_IsNotNull_Failed, this));
        }

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotNull(string errorMessage)
        {
            return Satisfies(IsNotNull, errorMessage);
        }

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotNull(string errorMessageFormat, object arg0)
        {
            return Satisfies(IsNotNull, errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>     
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>  
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotNull(string errorMessageFormat, object arg0, object arg1)
        {
            return Satisfies(IsNotNull, errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies whether or not this member is not <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotNull(string errorMessageFormat, params object[] arguments)
        {
            return Satisfies(IsNotNull, errorMessageFormat, arguments);
        }

        private static bool IsNotNull(TValue value)
        {
            return !ReferenceEquals(value, null);
        }

        #endregion

        #region [====== IsNull ======]

        /// <summary>
        /// Verifies whether or not this member is <c>null</c>.
        /// </summary>        
        public void IsNull()
        {
            Satisfies(IsNull, new ErrorMessage(ValidationMessages.Member_IsNull_Failed, this));
        }

        /// <summary>
        /// Verifies whether or not this member is <c>null</c>.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public void IsNull(string errorMessage)
        {
            Satisfies(IsNull, errorMessage);
        }

        /// <summary>
        /// Verifies whether or not this member is <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public void IsNull(string errorMessageFormat, object arg0)
        {
            Satisfies(IsNull, errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies whether or not this member is <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>     
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>          
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public void IsNull(string errorMessageFormat, object arg0, object arg1)
        {
            Satisfies(IsNull, errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies whether or not this member is <c>null</c>.
        /// </summary>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public void IsNull(string errorMessageFormat, params object[] arguments)
        {
            Satisfies(IsNull, errorMessageFormat, arguments);
        }

        private static bool IsNull(TValue value)
        {
            return ReferenceEquals(value, null);
        }

        #endregion

        #region [====== IsNotInstanceOf ======]

        /// <summary>
        /// Verifies that this member is not an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>        
        /// <returns>This member.</returns>        
        public Member<TValue> IsNotInstanceOf<TOther>()
        {
            return IsNotInstanceOf(typeof(TOther), new ErrorMessage(ValidationMessages.Member_IsNotInstanceOf_Failed, this, typeof(TOther)));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf<TOther>(string errorMessage)
        {
            return IsNotInstanceOf(typeof(TOther), new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf<TOther>(string errorMessageFormat, object arg0)
        {
            return IsNotInstanceOf(typeof(TOther), new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>     
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>  
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf<TOther>(string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotInstanceOf(typeof(TOther), new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf<TOther>(string errorMessageFormat, params object[] arguments)
        {
            return IsNotInstanceOf(typeof(TOther), new ErrorMessage(errorMessageFormat, arguments));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf(Type type)
        {
            return IsNotInstanceOf(type, new ErrorMessage(ValidationMessages.Member_IsNotInstanceOf_Failed, this, type));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf(Type type, string errorMessage)
        {
            return IsNotInstanceOf(type, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf(Type type, string errorMessageFormat, object arg0)
        {
            return IsNotInstanceOf(type, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>     
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>  
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf(Type type, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotInstanceOf(type, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf(Type type, string errorMessageFormat, params object[] arguments)
        {
            return IsNotInstanceOf(type, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsNotInstanceOf(Type type, ErrorMessage errorMessage)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsNotInstanceOf(value, type), errorMessage);
        }

        private static bool IsNotInstanceOf(TValue value, Type type)
        {
            return !IsInstanceOf(value, type);
        }

        #endregion

        #region [====== IsInstanceOf ======]

        /// <summary>
        /// Verifies that this member is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>        
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>        
        public Member<TOther> IsInstanceOf<TOther>()
        {
            return IsInstanceOf<TOther>(new ErrorMessage(ValidationMessages.Member_IsInstanceOf_Failed, this, typeof(TOther)));
        }

        /// <summary>
        /// Verifies that this member is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TOther> IsInstanceOf<TOther>(string errorMessage)
        {
            return IsInstanceOf<TOther>(new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>             
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TOther> IsInstanceOf<TOther>(string errorMessageFormat, object arg0)
        {
            return IsInstanceOf<TOther>(new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>     
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>  
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TOther> IsInstanceOf<TOther>(string errorMessageFormat, object arg0, object arg1)
        {
            return IsInstanceOf<TOther>(new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>             
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TOther> IsInstanceOf<TOther>(string errorMessageFormat, params object[] arguments)
        {
            return IsInstanceOf<TOther>(new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TOther> IsInstanceOf<TOther>(ErrorMessage errorMessage)
        {
            return Satisfies(value => IsInstanceOf(value, typeof(TOther)), errorMessage, value => (TOther) (object) value);
        }

        /// <summary>
        /// Verifies that this member is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsInstanceOf(Type type)
        {
            return IsInstanceOf(type, new ErrorMessage(ValidationMessages.Member_IsInstanceOf_Failed, this, type));
        }  

        /// <summary>
        /// Verifies that this member is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsInstanceOf(Type type, string errorMessage)
        {
            return IsInstanceOf(type, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsInstanceOf(Type type, string errorMessageFormat, object arg0)
        {
            return IsInstanceOf(type, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsInstanceOf(Type type, string errorMessageFormat, object arg0, object arg1)
        {
            return IsInstanceOf(type, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsInstanceOf(Type type, string errorMessageFormat, params object[] arguments)
        {
            return IsInstanceOf(type, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsInstanceOf(Type type, ErrorMessage errorMessage)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsInstanceOf(value, type), errorMessage);
        }

        private static bool IsInstanceOf(TValue value, Type type)
        {            
            return type.IsInstanceOfType(value);
        }

        #endregion  

        #region [====== IsNotSameInstanceAs ======]

        /// <summary>
        /// Verifies that this member does not refer to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>        
        /// <returns>This member.</returns>        
        public Member<TValue> IsNotSameInstanceAs(object other)
        {
            return IsNotSameInstanceAs(other, new ErrorMessage(ValidationMessages.Member_IsNotSameInstanceAs_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member does not refer to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotSameInstanceAs(object other, string errorMessage)
        {
            return IsNotSameInstanceAs(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member does not refer to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param> 
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotSameInstanceAs(object other, string errorMessageFormat, object arg0)
        {
            return IsNotSameInstanceAs(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member does not refer to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param> 
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param> 
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotSameInstanceAs(object other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotSameInstanceAs(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member does not refer to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param> 
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotSameInstanceAs(object other, string errorMessageFormat, params object[] arguments)
        {
            return IsNotSameInstanceAs(other, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsNotSameInstanceAs(object other, ErrorMessage errorMessage)
        {
            return Satisfies(value => IsNotSameInstanceAs(value, other), errorMessage);
        }

        private static bool IsNotSameInstanceAs(TValue value, object other)
        {
            return !ReferenceEquals(value, other);
        }

        #endregion
        
        #region [====== IsSameInstanceAs ======]

        /// <summary>
        /// Verifies that this member refers to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>        
        /// <returns>This member.</returns>        
        public Member<TValue> IsSameInstanceAs(object other)
        {
            return IsSameInstanceAs(other, new ErrorMessage(ValidationMessages.Member_IsSameInstanceAs_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member refers to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSameInstanceAs(object other, string errorMessage)
        {
            return IsSameInstanceAs(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member refers to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param> 
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSameInstanceAs(object other, string errorMessageFormat, object arg0)
        {
            return IsSameInstanceAs(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member refers to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param> 
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param> 
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSameInstanceAs(object other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsSameInstanceAs(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member refers to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's reference to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param> 
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSameInstanceAs(object other, string errorMessageFormat, params object[] arguments)
        {
            return IsSameInstanceAs(other, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsSameInstanceAs(object other, ErrorMessage errorMessage)
        {
            return Satisfies(value => IsSameInstanceAs(value, other), errorMessage);
        }

        private static bool IsSameInstanceAs(TValue value, object other)
        {
            return ReferenceEquals(value, other);
        }

        #endregion

        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>               
        /// <returns>This member.</returns>        
        public Member<TValue> IsNotEqualTo(object other)
        {
            return IsNotEqualTo(other, new ErrorMessage(ValidationMessages.Member_IsNotEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(object other, string errorMessage)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(object other, string errorMessageFormat, object arg0)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(object other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(object other, string errorMessageFormat, params object[] arguments)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessageFormat, arguments));
        }        
       
        private Member<TValue> IsNotEqualTo(object other, ErrorMessage errorMessage)
        {
            return Satisfies(value => IsNotEqual(value, other), errorMessage);
        }

        private static bool IsNotEqual(TValue value, object other)
        {
            return !IsEqual(value, other);
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>            
        /// <returns>This member.</returns>           
        public Member<TValue> IsNotEqualTo(TValue other, IEqualityComparer<TValue> comparer)
        {
            return IsNotEqualTo(new Equatable<TValue>(other, comparer), new ErrorMessage(ValidationMessages.Member_IsNotEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>This member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessage)
        {
            return IsNotEqualTo(new Equatable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsNotEqualTo(new Equatable<TValue>(other, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>    
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotEqualTo(new Equatable<TValue>(other, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsNotEqualTo(new Equatable<TValue>(other, comparer), errorMessageFormat, arguments);
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>                   
        /// <returns>This member.</returns>             
        public Member<TValue> IsNotEqualTo(IEquatable<TValue> other)
        {
            return IsNotEqualTo(other, new ErrorMessage(ValidationMessages.Member_IsNotEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>   
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(IEquatable<TValue> other, string errorMessage)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(IEquatable<TValue> other, string errorMessageFormat, object arg0)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(IEquatable<TValue> other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotEqualTo(IEquatable<TValue> other, string errorMessageFormat, params object[] arguments)
        {
            return IsNotEqualTo(other, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsNotEqualTo(IEquatable<TValue> other, ErrorMessage errorMessage)
        {
            return Satisfies(value => IsNotEqual(value, other), errorMessage);
        }

        private static bool IsNotEqual(TValue value, IEquatable<TValue> other)
        {
            return !IsEqual(value, other);
        }

        #endregion

        #region [====== IsEqualTo ======]

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>          
        /// <returns>This member.</returns>             
        public Member<TValue> IsEqualTo(object other)
        {
            return IsEqualTo(other, new ErrorMessage(ValidationMessages.Member_IsEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>   
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(object other, string errorMessage)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(object other, string errorMessageFormat, object arg0)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(object other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(object other, string errorMessageFormat, params object[] arguments)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessageFormat, arguments));
        }   
        
        private Member<TValue> IsEqualTo(object other, ErrorMessage errorMessage)
        {
            return Satisfies(value => IsEqual(value, other), errorMessage);
        }

        private static bool IsEqual(TValue value, object other)
        {
            return Equals(value, other);
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>                
        /// <returns>This member.</returns>        
        public Member<TValue> IsEqualTo(TValue other, IEqualityComparer<TValue> comparer)
        {
            return IsEqualTo(new Equatable<TValue>(other, comparer), new ErrorMessage(ValidationMessages.Member_IsEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessage)
        {
            return IsEqualTo(new Equatable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsEqualTo(new Equatable<TValue>(other, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsEqualTo(new Equatable<TValue>(other, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(TValue other, IEqualityComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsEqualTo(new Equatable<TValue>(other, comparer), errorMessageFormat, arguments);
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>                 
        /// <returns>This member.</returns>               
        public Member<TValue> IsEqualTo(IEquatable<TValue> other)
        {
            return IsEqualTo(other, new ErrorMessage(ValidationMessages.Member_IsEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(IEquatable<TValue> other, string errorMessage)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(IEquatable<TValue> other, string errorMessageFormat, object arg0)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(IEquatable<TValue> other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsEqualTo(IEquatable<TValue> other, string errorMessageFormat, params object[] arguments)
        {
            return IsEqualTo(other, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsEqualTo(IEquatable<TValue> other, ErrorMessage errorMessage)
        {
            return Satisfies(value => IsEqual(value, other), errorMessage);
        }

        private static bool IsEqual(TValue value, IEquatable<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return ReferenceEquals(value, null);
            }
            return other.Equals(value);
        }

        #endregion        

        #region [====== IsSmallerThan ======]

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>         
        /// <returns>This member.</returns>              
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThan(TValue other, IComparer<TValue> comparer)
        {
            return IsSmallerThan(new Comparable<TValue>(other, comparer), new ErrorMessage(ValidationMessages.Member_IsSmallerThan_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>This member.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThan(TValue other, IComparer<TValue> comparer, string errorMessage)
        {
            return IsSmallerThan(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThan(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsSmallerThan(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThan(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsSmallerThan(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThan(TValue other, IComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsSmallerThan(new Comparable<TValue>(other, comparer), errorMessageFormat, arguments);
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>                
        /// <returns>This member.</returns>              
        public Member<TValue> IsSmallerThan(IComparable<TValue> other)
        {
            return IsSmallerThan(other, new ErrorMessage(ValidationMessages.Member_IsSmallerThan_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>This member.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThan(IComparable<TValue> other, string errorMessage)
        {
            return IsSmallerThan(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThan(IComparable<TValue> other, string errorMessageFormat, object arg0)
        {
            return IsSmallerThan(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThan(IComparable<TValue> other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsSmallerThan(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThan(IComparable<TValue> other, string errorMessageFormat, params object[] arguments)
        {
            return IsSmallerThan(other, new ErrorMessage(errorMessageFormat, arguments));
        }  

        private Member<TValue> IsSmallerThan(IComparable<TValue> other, ErrorMessage errorMessage)
        {
            return Satisfies(value => Comparer.IsSmallerThan(value, other), errorMessage);            
        }             

        #endregion

        #region [====== IsSmallerThanOrEqualTo ======]

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>           
        /// <returns>This member.</returns>            
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(TValue other, IComparer<TValue> comparer)
        {
            return IsSmallerThanOrEqualTo(new Comparable<TValue>(other, comparer), new ErrorMessage(ValidationMessages.Member_IsSmallerThanOrEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>This member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessage)
        {
            return IsSmallerThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsSmallerThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsSmallerThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsSmallerThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessageFormat, arguments);
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>                       
        /// <returns>This member.</returns>        
        public Member<TValue> IsSmallerThanOrEqualTo(IComparable<TValue> other)
        {
            return IsSmallerThanOrEqualTo(other, new ErrorMessage(ValidationMessages.Member_IsSmallerThanOrEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(IComparable<TValue> other, string errorMessage)
        {
            return IsSmallerThanOrEqualTo(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(IComparable<TValue> other, string errorMessageFormat, object arg0)
        {
            return IsSmallerThanOrEqualTo(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(IComparable<TValue> other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsSmallerThanOrEqualTo(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsSmallerThanOrEqualTo(IComparable<TValue> other, string errorMessageFormat, params object[] arguments)
        {
            return IsSmallerThanOrEqualTo(other, new ErrorMessage(errorMessageFormat, arguments));
        }
        
        private Member<TValue> IsSmallerThanOrEqualTo(IComparable<TValue> other, ErrorMessage errorMessage)
        {
            return Satisfies(value => Comparer.IsSmallerThanOrEqualTo(value, other), errorMessage);           
        }        

        #endregion

        #region [====== IsGreaterThan ======]

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>         
        /// <returns>This member.</returns>            
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThan(TValue other, IComparer<TValue> comparer)
        {
            return IsGreaterThan(new Comparable<TValue>(other, comparer), new ErrorMessage(ValidationMessages.Member_IsGreaterThan_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>This member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThan(TValue other, IComparer<TValue> comparer, string errorMessage)
        {
            return IsGreaterThan(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThan(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsGreaterThan(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThan(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsGreaterThan(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThan(TValue other, IComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsGreaterThan(new Comparable<TValue>(other, comparer), errorMessageFormat, arguments);
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>                   
        /// <returns>This member.</returns>                  
        public Member<TValue> IsGreaterThan(IComparable<TValue> other)
        {
            return IsGreaterThan(other, new ErrorMessage(ValidationMessages.Member_IsGreaterThan_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>This member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>        
        public Member<TValue> IsGreaterThan(IComparable<TValue> other, string errorMessage)
        {
            return IsGreaterThan(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsGreaterThan(IComparable<TValue> other, string errorMessageFormat, object arg0)
        {
            return IsGreaterThan(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsGreaterThan(IComparable<TValue> other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsGreaterThan(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsGreaterThan(IComparable<TValue> other, string errorMessageFormat, params object[] arguments)
        {
            return IsGreaterThan(other, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsGreaterThan(IComparable<TValue> other, ErrorMessage errorMessage)
        {
            return Satisfies(value => Comparer.IsGreaterThan(value, other), errorMessage);            
        }        

        #endregion

        #region [====== IsGreaterThanOrEqualTo ======]

        /// <summary>
        /// Verifies that this member is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>               
        /// <returns>This member.</returns>        
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(TValue other, IComparer<TValue> comparer)
        {
            return IsGreaterThanOrEqualTo(new Comparable<TValue>(other, comparer), new ErrorMessage(ValidationMessages.Member_IsGreaterThanOrEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessage)
        {
            return IsGreaterThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsGreaterThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsGreaterThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>        
        public Member<TValue> IsGreaterThanOrEqualTo(TValue other, IComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsGreaterThanOrEqualTo(new Comparable<TValue>(other, comparer), errorMessageFormat, arguments);
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>                
        /// <returns>This member.</returns>              
        public Member<TValue> IsGreaterThanOrEqualTo(IComparable<TValue> other)
        {
            return IsGreaterThanOrEqualTo(other, new ErrorMessage(ValidationMessages.Member_IsGreaterThanOrEqualTo_Failed, this, other));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>This member.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(IComparable<TValue> other, string errorMessage)
        {
            return IsGreaterThanOrEqualTo(other, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(IComparable<TValue> other, string errorMessageFormat, object arg0)
        {
            return IsGreaterThanOrEqualTo(other, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(IComparable<TValue> other, string errorMessageFormat, object arg0, object arg1)
        {
            return IsGreaterThanOrEqualTo(other, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to compare this member's value to.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsGreaterThanOrEqualTo(IComparable<TValue> other, string errorMessageFormat, params object[] arguments)
        {
            return IsGreaterThanOrEqualTo(other, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsGreaterThanOrEqualTo(IComparable<TValue> other, ErrorMessage errorMessage)
        {
            return Satisfies(value => Comparer.IsGreaterThanOrEqualTo(value, other), errorMessage);            
        }       

        #endregion

        #region [====== IsNotInRange (TValue, TValue) ======]

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>                    
        /// <returns>This member.</returns>            
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>This member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, string errorMessage)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, string errorMessageFormat, object arg0)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, string errorMessageFormat, params object[] arguments)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsNotInRange (TValue, TValue, RangeOptions) ======]

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>                 
        /// <returns>This member.</returns>              
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, RangeOptions options)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, null, options));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>This member.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, RangeOptions options, string errorMessage)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, null, options), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, RangeOptions options, string errorMessageFormat, object arg0)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, null, options), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, RangeOptions options, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, options), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, RangeOptions options, string errorMessageFormat, params object[] arguments)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, options), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsNotInRange (TValue, TValue, IComparer<TValue>) ======]

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>        
        /// <returns>This member.</returns>               
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessage)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsNotInRange (TValue, TValue, IComparer<TValue>, RangeOptions) ======]

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>         
        /// <returns>This member.</returns>               
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer, options));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessageFormat, object arg0)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsNotInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessageFormat, params object[] arguments)
        {
            return IsNotInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsNotInRange (IRange<TValue>) ======]

        /// <summary>
        /// Verifies that this member's value does not lie within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>        
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>        
        public Member<TValue> IsNotInRange(IRange<TValue> range)
        {
            return IsNotInRange(range, new ErrorMessage(ValidationMessages.Member_IsNotInRange_Failed, this, range));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>        
        public Member<TValue> IsNotInRange(IRange<TValue> range, string errorMessage)
        {
            return IsNotInRange(range, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsNotInRange(IRange<TValue> range, string errorMessageFormat, object arg0)
        {
            return IsNotInRange(range, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsNotInRange(IRange<TValue> range, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotInRange(range, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member's value does not lie within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsNotInRange(IRange<TValue> range, string errorMessageFormat, params object[] arguments)
        {
            return IsNotInRange(range, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsNotInRange(IRange<TValue> range, ErrorMessage errorMessage)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            return Satisfies(value => IsNotInRange(value, range), errorMessage);
        }

        private static bool IsNotInRange(TValue value, IRange<TValue> range)
        {
            return !IsInRange(value, range);
        }

        #endregion

        #region [====== IsInRange (TValue, TValue) ======]

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>                   
        /// <returns>This member.</returns>            
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right)
        {
            return IsInRange(new InternalRange<TValue>(left, right));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>This member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, string errorMessage)
        {
            return IsInRange(new InternalRange<TValue>(left, right), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, string errorMessageFormat, object arg0)
        {
            return IsInRange(new InternalRange<TValue>(left, right), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, string errorMessageFormat, object arg0, object arg1)
        {
            return IsInRange(new InternalRange<TValue>(left, right), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, string errorMessageFormat, params object[] arguments)
        {
            return IsInRange(new InternalRange<TValue>(left, right), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsInRange (TValue, TValue, RangeOptions) ======]

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>                  
        /// <returns>This member.</returns>              
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, RangeOptions options)
        {
            return IsInRange(new InternalRange<TValue>(left, right, null, options));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>This member.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, RangeOptions options, string errorMessage)
        {
            return IsInRange(new InternalRange<TValue>(left, right, null, options), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, RangeOptions options, string errorMessageFormat, object arg0)
        {
            return IsInRange(new InternalRange<TValue>(left, right, null, options), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, RangeOptions options, string errorMessageFormat, object arg0, object arg1)
        {
            return IsInRange(new InternalRange<TValue>(left, right, options), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>        
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, RangeOptions options, string errorMessageFormat, params object[] arguments)
        {
            return IsInRange(new InternalRange<TValue>(left, right, options), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsInRange (TValue, TValue, IComparer<TValue>) ======]

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>        
        /// <returns>This member.</returns>               
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessage)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessageFormat, object arg0)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessageFormat, object arg0, object arg1)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, string errorMessageFormat, params object[] arguments)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsInRange (TValue, TValue, IComparer<TValue>, RangeOptions) ======]

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param>         
        /// <returns>This member.</returns>               
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer, options));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessage);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessageFormat, object arg0)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessageFormat, arg0);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessageFormat, object arg0, object arg1)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessageFormat, arg0, arg1);
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified range.
        /// </summary>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> ar part of the range themselves.
        /// </param> 
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// </exception>
        public Member<TValue> IsInRange(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessageFormat, params object[] arguments)
        {
            return IsInRange(new InternalRange<TValue>(left, right, comparer, options), errorMessageFormat, arguments);
        }

        #endregion

        #region [====== IsInRange (IRange<TValue>) ======]

        /// <summary>
        /// Verifies that this member's value lies within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>        
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsInRange(IRange<TValue> range)
        {
            return IsInRange(range, new ErrorMessage(ValidationMessages.Member_IsIsRange_Failed, this, range));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsInRange(IRange<TValue> range, string errorMessage)
        {
            return IsInRange(range, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsInRange(IRange<TValue> range, string errorMessageFormat, object arg0)
        {
            return IsInRange(range, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsInRange(IRange<TValue> range, string errorMessageFormat, object arg0, object arg1)
        {
            return IsInRange(range, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member's value lies within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> IsInRange(IRange<TValue> range, string errorMessageFormat, params object[] arguments)
        {
            return IsInRange(range, new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TValue> IsInRange(IRange<TValue> range, ErrorMessage errorMessage)
        {           
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            return Satisfies(value => IsInRange(value, range), errorMessage);
        }

        private static bool IsInRange(TValue value, IRange<TValue> range)
        {
            return range.Contains(value);
        }        

        #endregion

        #region [====== Satisfies ======]

        /// <summary>
        /// Verifies that this member satisfies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">A constraint or predicate for this member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>This member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> Satisfies(Func<TValue, bool> constraint, string errorMessage)
        {            
            return Satisfies(constraint, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that this member satisfies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">A constraint or predicate for this member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>                     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> Satisfies(Func<TValue, bool> constraint, string errorMessageFormat, object arg0)
        {
            return Satisfies(constraint, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that this member satisfies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">A constraint or predicate for this member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>                     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> Satisfies(Func<TValue, bool> constraint, string errorMessageFormat, object arg0, object arg1)
        {
            return Satisfies(constraint, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that this member satisfies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">A constraint or predicate for this member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>This member.</returns>                     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> Satisfies(Func<TValue, bool> constraint, string errorMessageFormat, params object[] arguments)
        {
            return Satisfies(constraint, new ErrorMessage(errorMessageFormat, arguments));
        }

        /// <summary>
        /// Verifies that this member satisfies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">A constraint or predicate for this member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>                     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> Satisfies(Func<TValue, bool> constraint, ErrorMessage errorMessage)
        {            
            _constraint = _constraint.And(this, constraint, errorMessage, _memberSet.Consumer);            
            return this;
        }        

        /// <summary>
        /// Verifies that this member satisfies the specified <paramref name="constraint"/> and converts this member into
        /// an instance of <see cref="Member{TOther}" />.
        /// </summary>
        /// <typeparam name="TOther">Type of the converted value.</typeparam>
        /// <param name="constraint">A constraint or predicate for this member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <param name="selector">The delegate that is used to convert <see cref="Value" /> into an instance of <typeparamref name="TOther"/>.</param> 
        /// <param name="newMemberName">If not <c>null</c>, specifies the name of the member that is returned by this method.</param>  
        /// <returns>This member.</returns>                     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/>, <paramref name="errorMessage"/> or <paramref name="selector"/> is <c>null</c>.
        /// </exception> 
        public Member<TOther> Satisfies<TOther>(Func<TValue, bool> constraint, ErrorMessage errorMessage, Func<TValue, TOther> selector, string newMemberName = null)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
            var internalConstraint = _constraint.And(this, constraint, errorMessage, _memberSet.Consumer);
            var name = string.IsNullOrEmpty(newMemberName) ? _name : newMemberName;
            var member = new Member<TOther>(_memberSet, () => selector.Invoke(Value), name, _parentName, internalConstraint);

            _memberSet.Replace(this, member);

            return member;
        }

        #endregion                                          
    }
}
