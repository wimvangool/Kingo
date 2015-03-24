using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represent a certain member that can be validated and produces an <see cref="ErrorMessage" /> if this validation fails.
    /// </summary>
    /// <typeparam name="TValue">Type of the member's value.</typeparam>
    public class Member<TValue> : IMember
    {
        private readonly IMemberParent _parent;
        private readonly string _name;
        private readonly Lazy<TValue> _value;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Member{TValue}" /> class.
        /// </summary>
        /// <param name="parent">The parent of this member.</param>
        /// <param name="memberExpression">An expression that returns an instance of <typeparamref name="TValue"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parent"/> or <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        public Member(IMemberParent parent, Expression<Func<TValue>> memberExpression)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (memberExpression == null)
            {
                throw new ArgumentNullException("memberExpression");
            }
            _parent = parent;
            _name = ExtractMemberNameOf(memberExpression);
            _value = new Lazy<TValue>(memberExpression.Compile());

            Constraint = new NullConstraint();
        }

        internal Member(IMemberParent parent, string name, Func<TValue> valueProvider, Constraint constraint)
        {
            _parent = parent;
            _name = name;
            _value = new Lazy<TValue>(valueProvider);

            Constraint = constraint;
        }

        /// <summary>
        /// The parent of this member.
        /// </summary>
        protected IMemberParent Parent
        {
            get { return _parent; }
        }

        internal Constraint Constraint
        {
            get;
            private set;
        }

        #region [====== IMember & IErrorMessageProducer ======]

        string IMember.Name
        {
            get { return Name; }
        }

        object IMember.Value
        {
            get { return Value; }
        }

        /// <summary>
        /// The name of this member.
        /// </summary>
        protected internal string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The value of this member.
        /// </summary>
        protected internal TValue Value
        {
            get { return _value.Value; }
        }

        int IErrorMessageProducer.Accept(IErrorMessageConsumer consumer)
        {
            return Constraint.Accept(consumer);
        }

        private static string ExtractMemberNameOf(Expression valueExpression)
        {            
            var lambdaExpression = (LambdaExpression) valueExpression;
            var unaryExpression = lambdaExpression.Body as UnaryExpression;
            var memberExpression = unaryExpression == null
                ? (MemberExpression) lambdaExpression.Body
                : (MemberExpression) unaryExpression.Operand;

            return memberExpression.Member.Name;
        }

        #endregion

        #region [====== IsNotNull ======]

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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TValue> IsNotInstanceOf<TOther>(string errorMessage)
        {
            return Satisfies(value => IsNotInstanceOf(value, typeof(TOther)), errorMessage);
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
            return Satisfies(value => IsNotInstanceOf(value, typeof(TOther)), errorMessageFormat, arg0);
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
            return Satisfies(value => IsNotInstanceOf(value, typeof(TOther)), errorMessageFormat, arg0, arg1);
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
            return Satisfies(value => IsNotInstanceOf(value, typeof(TOther)), errorMessageFormat, arguments);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsNotInstanceOf(value, type), errorMessage);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsNotInstanceOf(value, type), errorMessageFormat, arg0);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsNotInstanceOf(value, type), errorMessageFormat, arg0, arg1);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsNotInstanceOf(value, type), errorMessageFormat, arguments);
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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public Member<TOther> IsInstanceOf<TOther>(string errorMessage)
        {
            return CastMemberTo<TOther>(new ErrorMessage(errorMessage));
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
            return CastMemberTo<TOther>(new ErrorMessage(errorMessageFormat, arg0));
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
            return CastMemberTo<TOther>(new ErrorMessage(errorMessageFormat, arg0, arg1));
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
            return CastMemberTo<TOther>(new ErrorMessage(errorMessageFormat, arguments));
        }

        private Member<TOther> CastMemberTo<TOther>(ErrorMessage errorMessage)
        {
            var constraint = IsInstanceOfConstraint(typeof(TOther), errorMessage);
            var member = new Member<TOther>(Parent, Name, CastValueTo<TOther>, constraint);

            Parent.ReplaceMember(_name, this, member);

            return member;
        }

        private TOther CastValueTo<TOther>()
        {
            return (TOther) (object) Value;
        }

        private Constraint IsInstanceOfConstraint(Type type, ErrorMessage errorMessage)
        {
            return Constraint.And(this, value => IsInstanceOf(value, type), errorMessage);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsInstanceOf(value, type), errorMessage);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsInstanceOf(value, type), errorMessageFormat, arg0);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsInstanceOf(value, type), errorMessageFormat, arg0, arg1);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return Satisfies(value => IsInstanceOf(value, type), errorMessageFormat, arguments);
        }

        private static bool IsInstanceOf(TValue value, Type type)
        {            
            return type.IsInstanceOfType(value);
        }

        #endregion                       

        #region [====== IsNotEqualTo ======]

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
            Constraint = Constraint.And(this, constraint, errorMessage);
            return this;
        }

        #endregion                       
    }
}
