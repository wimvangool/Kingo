using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceComponents.Resources;

namespace ServiceComponents.ComponentModel.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
    /// </summary>
    public static class MemberConstraints
    {
        /// <summary>
        /// Represents the canonical identifier used to identify the member inside an error format string.
        /// </summary>
        public const string MemberId = "member";

        /// <summary>
        /// Represents the canonical identifier used to identify the member inside an error format string.
        /// </summary>
        public const string ConstraintId = "constraint";

        #region [====== IsNotNull ======]

        /// <summary>
        /// Verifies whether or not the member's value is not <c>null</c>.
        /// </summary>     
        /// <param name="member">A member.</param>   
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsNotNull<TValue>(this IMemberConstraint<TValue> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotNullConstraint<TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not null.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotNullConstraint<TValue>(string errorMessage = null)
        {
            return New.Constraint<TValue>(value => !ReferenceEquals(value, null))
                .WithDisplayFormat("{member.Name} != null")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotNull)
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsNull ======]

        /// <summary>
        /// Verifies whether or not the member's value is <c>null</c>.
        /// </summary> 
        /// <param name="member">A member.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsNull<TValue>(this IMemberConstraint<TValue> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullConstraint<TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is null.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNullConstraint<TValue>(string errorMessage = null)
        {
            return New.Constraint<TValue>(value => ReferenceEquals(value, null))
                .WithDisplayFormat("{member.Name} == null")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNull)
                .BuildConstraint();
        }        

        #endregion        

        #region [====== IsNotSameInstanceAs ======]

        /// <summary>
        /// Verifies that the member's value does not refer to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>       
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsNotSameInstanceAs<TValue>(this IMemberConstraint<TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotSameInstanceAsConstraint<TValue>(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not the same instance as another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotSameInstanceAsConstraint<TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => !ReferenceEquals(value, other))
                .WithDisplayFormat("{member.Name} !== {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotSameInstanceAs)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsSameInstanceAs ======]

        /// <summary>
        /// Verifies that the member's value refers to the same instance as <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>   
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsSameInstanceAs<TValue>(this IMemberConstraint<TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSameInstanceAsConstraint<TValue>(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is the same instance as another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsSameInstanceAsConstraint<TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => ReferenceEquals(value, other))
                .WithDisplayFormat("{member.Name} === {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsSameInstanceAs)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsNotInstanceOf ======]

        /// <summary>
        /// Verifies that this member's value is not an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TValue> IsNotInstanceOf<TValue>(this IMemberConstraint<TValue> member, Type type, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInstanceOfConstraint<TValue>(type, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not of a specific type.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotInstanceOfConstraint<TValue>(Type type, string errorMessage = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return New.Constraint<TValue>(value => !type.IsInstanceOfType(value))
                .WithDisplayFormat("!({member.Name} is {member.Type.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotInstanceOf)
                .WithArguments(new { Type = type })
                .BuildConstraint();
        }

        #endregion

        #region [====== IsInstanceOf ======]

        /// <summary>
        /// Verifies that the member's value is an instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="type">The type to compare the member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TValue> IsInstanceOf<TValue>(this IMemberConstraint<TValue> member, Type type, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInstanceOfConstraint<TValue>(type, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is of a specific type.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsInstanceOfConstraint<TValue>(Type type, string errorMessage = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return New.Constraint<TValue>(value => type.IsInstanceOfType(value))
                .WithDisplayFormat("({member.Name} is {member.Type.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsInstanceOf)
                .WithArguments(new { Type = type })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is of a specific type.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <typeparam name="TOther">Type the value should be cast to.</typeparam>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>        
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TOther> IsInstanceOfConstraint<TValue, TOther>(string errorMessage = null)
        {            
            return New.Constraint<TValue, TOther>(value => value is TOther, value => (TOther) (object) value)
                .WithDisplayFormat("({member.Name} is {member.Type.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsInstanceOf)
                .WithArguments(new { Type = typeof(TOther) })
                .BuildConstraint();
        }

        #endregion

        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsNotEqualTo<TValue>(this IMemberConstraint<TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint<TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsNotEqualTo<TValue>(this IMemberConstraint<TValue> member, TValue other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint(other, errorMessage));
        }        

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>     
        public static IMemberConstraint<TValue> IsNotEqualTo<TValue>(this IMemberConstraint<TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint(other, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>           
        public static IMemberConstraint<TValue> IsNotEqualTo<TValue>(this IMemberConstraint<TValue> member, IEquatable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotEqualToConstraint<TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => !Equals(value, other))
                .WithDisplayFormat("{member.Name} != {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotEqualToConstraint<TValue>(TValue other, string errorMessage = null)
        {
            return IsNotEqualToConstraint(other, EqualityComparer<TValue>.Default, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotEqualToConstraint<TValue>(TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            return IsNotEqualToConstraint<TValue>(new Equatable<TValue>(other, comparer), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotEqualToConstraint<TValue>(IEquatable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => !Comparer.IsEqualTo(value, other))
                .WithDisplayFormat("{member.Name} != {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TValue> IsEqualTo<TValue>(this IMemberConstraint<TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint<TValue>(other, errorMessage));
        }        

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TValue> IsEqualTo<TValue>(this IMemberConstraint<TValue> member, TValue other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsEqualTo<TValue>(this IMemberConstraint<TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint(other, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>    
        public static IMemberConstraint<TValue> IsEqualTo<TValue>(this IMemberConstraint<TValue> member, IEquatable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsEqualToConstraint<TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => Equals(value, other))
                .WithDisplayFormat("{member.Name} == {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsEqualToConstraint<TValue>(TValue other, string errorMessage = null)
        {
            return IsEqualToConstraint(other, EqualityComparer<TValue>.Default, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsEqualToConstraint<TValue>(TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            return IsEqualToConstraint<TValue>(new Equatable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsEqualToConstraint<TValue>(IEquatable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => Comparer.IsEqualTo(value, other))
                .WithDisplayFormat("{member.Name} == {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsSmallerThan ======]

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsSmallerThan<TValue>(this IMemberConstraint<TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanConstraint(other, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> IsSmallerThan<TValue>(this IMemberConstraint<TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanConstraint(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is smaller than another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsSmallerThanConstraint<TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsSmallerThanConstraint<TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is ...
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>   
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsSmallerThanConstraint<TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => Comparer.IsSmallerThan(value, other))
                .WithDisplayFormat("{member.Name} < {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThan)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsSmallerThanOrEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsSmallerThanOrEqualTo<TValue>(this IMemberConstraint<TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanOrEqualToConstraint(other, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TValue> IsSmallerThanOrEqualTo<TValue>(this IMemberConstraint<TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanOrEqualToConstraint(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is smaller than or equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsSmallerThanOrEqualToConstraint<TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsSmallerThanOrEqualToConstraint<TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is ...
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsSmallerThanOrEqualToConstraint<TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => Comparer.IsSmallerThanOrEqualTo(value, other))
                .WithDisplayFormat("{member.Name} <= {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThanOrEqualTo)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsGreaterThan ======]

        /// <summary>
        /// Verifies that the member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsGreaterThan<TValue>(this IMemberConstraint<TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanConstraint(other, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TValue> IsGreaterThan<TValue>(this IMemberConstraint<TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanConstraint(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is greater than another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsGreaterThanConstraint<TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsGreaterThanConstraint<TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is greater than another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsGreaterThanConstraint<TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => Comparer.IsGreaterThan(value, other))
                .WithDisplayFormat("{member.Name} > {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThan)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsGreaterThanOrEqualTo ======]

        /// <summary>
        /// Verifies that the member is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="other"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsGreaterThanOrEqualTo<TValue>(this IMemberConstraint<TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanOrEqualToConstraint(other, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>     
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        public static IMemberConstraint<TValue> IsGreaterThanOrEqualTo<TValue>(this IMemberConstraint<TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanOrEqualToConstraint(other, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is ...
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsGreaterThanOrEqualToConstraint<TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsGreaterThanOrEqualToConstraint<TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is greater than or equal to another value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsGreaterThanOrEqualToConstraint<TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TValue>(value => Comparer.IsGreaterThanOrEqualTo(value, other))
                .WithDisplayFormat("{member.Name} >= {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThanOrEqualTo)
                .WithArguments(new { Other = other })
                .BuildConstraint();
        }

        #endregion

        #region [====== IsNotInRange ======]

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsNotInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(left, right, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsNotInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(left, right, options, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsNotInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(left, right, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>              
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsNotInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(left, right, comparer, options, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TValue> IsNotInRange<TValue>(this IMemberConstraint<TValue> member, IRange<TValue> range, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsNotInRangeConstraint(range, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotInRangeConstraint<TValue>(TValue left, TValue right, string errorMessage = null)
        {
            return IsNotInRangeConstraint(new RangeAdapter<TValue>(left, right), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotInRangeConstraint<TValue>(TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            return IsNotInRangeConstraint(new RangeAdapter<TValue>(left, right, null, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotInRangeConstraint<TValue>(TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsNotInRangeConstraint(new RangeAdapter<TValue>(left, right, comparer), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotInRangeConstraint<TValue>(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            return IsNotInRangeConstraint(new RangeAdapter<TValue>(left, right, comparer, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsNotInRangeConstraint<TValue>(IRange<TValue> range, string errorMessage = null)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }                       
            return New.Constraint<TValue>(value => !range.Contains(value))
                .WithDisplayFormat("!({member.Name} in {member.Range}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotInRange)
                .WithArguments(new { Range = range })
                .BuildConstraint();
        }

        #endregion

        #region [====== IsInRange ======]

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces 
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(left, right, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(left, right, options, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(left, right, comparer, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TValue> IsInRange<TValue>(this IMemberConstraint<TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(left, right, comparer, options, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="range"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        public static IMemberConstraint<TValue> IsInRange<TValue>(this IMemberConstraint<TValue> member, IRange<TValue> range, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsInRangeConstraint(range, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsInRangeConstraint<TValue>(TValue left, TValue right, string errorMessage = null)
        {
            return IsInRangeConstraint(new RangeAdapter<TValue>(left, right), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsInRangeConstraint<TValue>(TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            return IsInRangeConstraint(new RangeAdapter<TValue>(left, right, null, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsInRangeConstraint<TValue>(TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsInRangeConstraint(new RangeAdapter<TValue>(left, right, comparer), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsInRangeConstraint<TValue>(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            return IsInRangeConstraint(new RangeAdapter<TValue>(left, right, comparer, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue, TValue> IsInRangeConstraint<TValue>(IRange<TValue> range, string errorMessage = null)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }            
            return New.Constraint<TValue>(range.Contains)
                .WithDisplayFormat("!({member.Name} in {member.Range}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsInRange)
                .WithArguments(new { Range = range })
                .BuildConstraint();
        }       

        #endregion        
    }
}
