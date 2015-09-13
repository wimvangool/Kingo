using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static class MemberConstraints
    {
        /// <summary>
        /// Represents the canonical identifier used to identify the <i>member</i> placeholder(s) inside an error format string.
        /// </summary>
        public const string MemberId = "member";        

        /// <summary>
        /// Represents the canonical identifier used to identify the <i>constraint</i> placeholder(s) inside an error format string.
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
        public static IMemberConstraint<TMessage, TValue> IsNotNull<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotNullConstraint<TMessage, TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not null.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotNullConstraint<TMessage, TValue>(string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => !ReferenceEquals(member, null))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotNull)
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
        public static IMemberConstraint<TMessage, TValue> IsNull<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullConstraint<TMessage, TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is null.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNullConstraint<TMessage, TValue>(string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => ReferenceEquals(member, null))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNull)
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
        public static IMemberConstraint<TMessage, TValue> IsNotSameInstanceAs<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotSameInstanceAsConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not refer to the same instance as <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>       
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotSameInstanceAs<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsNotSameInstanceAsConstraint<TMessage, TValue>(otherFactory, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not the same instance as another value.
        /// </summary>       
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotSameInstanceAsConstraint<TMessage, TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => !ReferenceEquals(member, other))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotSameInstanceAs)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();            
        } 

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not the same instance as another value.
        /// </summary>       
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's reference to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotSameInstanceAsConstraint<TMessage, TValue>(Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();
            
            return New.Constraint<TMessage, TValue>((member, message) => !ReferenceEquals(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotSameInstanceAs)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsSameInstanceAs<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSameInstanceAsConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value refers to the same instance as <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's reference to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>   
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsSameInstanceAs<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSameInstanceAsConstraint<TMessage, TValue>(otherFactory, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not the same instance as another value.
        /// </summary>       
        /// <param name="other">The instance to compare the member's reference to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSameInstanceAsConstraint<TMessage, TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => ReferenceEquals(member, other))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSameInstanceAs)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not the same instance as another value.
        /// </summary>       
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's reference to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSameInstanceAsConstraint<TMessage, TValue>(Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => ReferenceEquals(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSameInstanceAs)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsNotInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Type type, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInstanceOfConstraint<TMessage, TValue>(type, errorMessage));
        }

        /// <summary>
        /// Verifies that this member's value is not an instance of <paramref name="typeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="typeFactory">Delegate that returns the type to compare this member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsNotInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, Type> typeFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInstanceOfConstraint<TMessage, TValue>(typeFactory, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not of a specific type.
        /// </summary>        
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInstanceOfConstraint<TMessage, TValue>(Type type, string errorMessage = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return New.Constraint<TMessage, TValue>(member => !type.IsInstanceOfType(member))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotInstanceOf)
                .WithErrorMessageArguments(new { Type = type })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not of a specific type.
        /// </summary>        
        /// <param name="typeFactory">The type to compare this member's type to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInstanceOfConstraint<TMessage, TValue>(Func<TMessage, Type> typeFactory, string errorMessage = null)
        {
            if (typeFactory == null)
            {
                throw new ArgumentNullException("typeFactory");
            }
            return New.Constraint<TMessage, TValue>((member, message) => !typeFactory.Invoke(message).IsInstanceOfType(member))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotInstanceOf)
                .WithErrorMessageArguments(message => new { Type = typeFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Type type, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInstanceOfConstraint<TMessage, TValue>(type, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is an instance of <paramref name="typeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="typeFactory">Delegate that returns the type to compare the member's type to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsInstanceOf<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, Type> typeFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInstanceOfConstraint<TMessage, TValue>(typeFactory, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is of a specific type.
        /// </summary>        
        /// <param name="type">The type to compare this member's type to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInstanceOfConstraint<TMessage, TValue>(Type type, string errorMessage = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return New.Constraint<TMessage, TValue>(member => type.IsInstanceOfType(member))            
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsInstanceOf)
                .WithErrorMessageArguments(new { Type = type })
                .BuildConstraint();
        }        

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is of a specific type.
        /// </summary>        
        /// <param name="typeFactory">Delegate that returns the type to compare this member's type to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="typeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInstanceOfConstraint<TMessage, TValue>(Func<TMessage, Type> typeFactory, string errorMessage = null)
        {
            if (typeFactory == null)
            {
                throw new ArgumentNullException("typeFactory");
            }
            return New.Constraint<TMessage, TValue>((member, message) => typeFactory.Invoke(message).IsInstanceOfType(member))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsInstanceOf)
                .WithErrorMessageArguments(message => new { Type = typeFactory.Invoke(message) })
                .BuildConstraint();            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is of a specific type.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>        
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TOther> IsInstanceOfConstraint<TMessage, TValue, TOther>(string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue, TOther>(member => member is TOther, member => (TOther) (object) member)           
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsInstanceOf)
                .WithErrorMessageArguments(new { Type = typeof(TOther) })
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint<TMessage, TValue>(other, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint<TMessage, TValue>(other, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint<TMessage, TValue>(other, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IEquatable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint<TMessage, TValue>(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>     
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint(otherFactory, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>           
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, IEquatable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotEqualToConstraint(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => !Equals(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(TValue other, string errorMessage = null)
        {
            return IsNotEqualToConstraint<TMessage, TValue>(other, EqualityComparer<TValue>.Default, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            return IsNotEqualToConstraint<TMessage, TValue>(new Equatable<TValue>(other, comparer), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(IEquatable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => !Comparer.IsEqualTo(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => !Equals(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();                        
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, string errorMessage = null)
        {
            return IsNotEqualToConstraint(otherFactory, EqualityComparer<TValue>.Default, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => !Comparer.IsEqualTo(member, new Equatable<TValue>(otherFactoryDelegate.Invoke(message), comparer)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();             
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, IEquatable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => !Comparer.IsEqualTo(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, object other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint<TMessage, TValue>(other, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint<TMessage, TValue>(other, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint<TMessage, TValue>(other, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IEquatable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint<TMessage, TValue>(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint(otherFactory, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">Delegate that returns the instance to compare the member's value to.</param>        
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
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, IEquatable<TValue>>> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsEqualToConstraint(other, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(object other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => Equals(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }        

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(TValue other, string errorMessage = null)
        {
            return IsEqualToConstraint<TMessage, TValue>(other, EqualityComparer<TValue>.Default, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(TValue other, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            return IsEqualToConstraint<TMessage, TValue>(new Equatable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(IEquatable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => Comparer.IsEqualTo(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, object>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Equals(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();  
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, string errorMessage = null)
        {
            return IsEqualToConstraint(otherFactory, EqualityComparer<TValue>.Default, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsEqualTo(member, new Equatable<TValue>(otherFactoryDelegate.Invoke(message), comparer)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();             
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, IEquatable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsEqualTo(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanConstraint<TMessage, TValue>(other, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">Delegate that returns the instance to compare the member's value to.</param>
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanConstraint(other, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is smaller than <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsSmallerThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanConstraint(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanConstraint<TMessage, TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsSmallerThanConstraint<TMessage, TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than <paramref name="other"/>.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>   
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanConstraint<TMessage, TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => Comparer.IsSmallerThan(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThan)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsSmallerThan(member, new Comparable<TValue>(otherFactoryDelegate.Invoke(message), comparer)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThan)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();              
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than <paramref name="otherFactory"/>.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>   
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanConstraint<TMessage, TValue>(Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsSmallerThan(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThan)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanOrEqualToConstraint<TMessage, TValue>(other, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanOrEqualToConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="otherFactory"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanOrEqualToConstraint(otherFactory, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value is smaller than or equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsSmallerThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSmallerThanOrEqualToConstraint(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than or equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanOrEqualToConstraint<TMessage, TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsSmallerThanOrEqualToConstraint<TMessage, TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than or equal to <paramref name="other"/>.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanOrEqualToConstraint<TMessage, TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => Comparer.IsSmallerThanOrEqualTo(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThanOrEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than or equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanOrEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsSmallerThanOrEqualTo(member, new Comparable<TValue>(otherFactoryDelegate.Invoke(message), comparer)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThanOrEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();             
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is smaller than or equal to <paramref name="otherFactory"/>.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsSmallerThanOrEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsSmallerThanOrEqualTo(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsSmallerThanOrEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsGreaterThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanConstraint<TMessage, TValue>(other, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsGreaterThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member is greater than <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="otherFactory"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsGreaterThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanConstraint(otherFactory, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member is greater than <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TMessage, TValue> IsGreaterThan<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanConstraint(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanConstraint<TMessage, TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsGreaterThanConstraint<TMessage, TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanConstraint<TMessage, TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => Comparer.IsGreaterThan(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThan)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsGreaterThan(member, new Comparable<TValue>(otherFactoryDelegate.Invoke(message), comparer)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThan)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();              
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanConstraint<TMessage, TValue>(Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsGreaterThan(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThan)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsGreaterThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanOrEqualToConstraint<TMessage, TValue>(other, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsGreaterThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IComparable<TValue> other, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanOrEqualToConstraint<TMessage, TValue>(other, errorMessage));
        }

        /// <summary>
        /// Verifies that the member is greater than or equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>        
        /// <returns>This member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer"/> is <c>null</c> and <paramref name="otherFactory"/> does not implement the
        /// <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsGreaterThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, TValue>> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanOrEqualToConstraint(otherFactory, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>     
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        public static IMemberConstraint<TMessage, TValue> IsGreaterThanOrEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsGreaterThanOrEqualToConstraint(otherFactory, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than or equal to <paramref name="other"/>.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanOrEqualToConstraint<TMessage, TValue>(TValue other, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsGreaterThanOrEqualToConstraint<TMessage, TValue>(new Comparable<TValue>(other, comparer), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than or equal to another value.
        /// </summary>        
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanOrEqualToConstraint<TMessage, TValue>(IComparable<TValue> other, string errorMessage = null)
        {
            return New.Constraint<TMessage, TValue>(member => Comparer.IsGreaterThanOrEqualTo(member, other))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThanOrEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than or equal to <paramref name="otherFactory"/>.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanOrEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, TValue>> otherFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsGreaterThanOrEqualTo(member, new Comparable<TValue>(otherFactoryDelegate.Invoke(message), comparer)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThanOrEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
                .BuildConstraint();             
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is greater than or equal to another value.
        /// </summary>        
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsGreaterThanOrEqualToConstraint<TMessage, TValue>(Expression<Func<TMessage, IComparable<TValue>>> otherFactory, string errorMessage = null)
        {
            if (otherFactory == null)
            {
                throw new ArgumentNullException("otherFactory");
            }
            var otherFactoryDelegate = otherFactory.Compile();

            return New.Constraint<TMessage, TValue>((member, message) => Comparer.IsGreaterThanOrEqualTo(member, otherFactoryDelegate.Invoke(message)))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsGreaterThanOrEqualTo)
                .WithErrorMessageArguments(message => new { Other = otherFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint<TMessage, TValue>(left, right, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint<TMessage, TValue>(left, right, options, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint<TMessage, TValue>(left, right, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint<TMessage, TValue>(left, right, comparer, options, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IRange<TValue> range, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint<TMessage, TValue>(range, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(leftFactory, rightFactory, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(leftFactory, rightFactory, options, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(leftFactory, rightFactory, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>              
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(leftFactory, rightFactory, comparer, options, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified <paramref name="rangeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="rangeFactory">Delegate that returns a range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="rangeFactory"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rangeFactory"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>  
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IRange<TValue>> rangeFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotInRangeConstraint(rangeFactory, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(TValue left, TValue right, string errorMessage = null)
        {
            return IsNotInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            return IsNotInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right, null, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsNotInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right, comparer), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            return IsNotInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right, comparer, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(IRange<TValue> range, string errorMessage = null)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            return New.Constraint<TMessage, TValue>(member => !range.Contains(member))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotInRange)
                .WithErrorMessageArguments(new { Range = range })
                .BuildConstraint();            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            return IsNotInRangeConstraint(leftFactory, rightFactory, null, RangeOptions.None, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that retuns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            return IsNotInRangeConstraint(leftFactory, rightFactory, null, options, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsNotInRangeConstraint(leftFactory, rightFactory, comparer, RangeOptions.None, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return IsNotInRangeConstraint<TMessage, TValue>(message => new RangeAdapter<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), comparer, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is not within a certain range.
        /// </summary>        
        /// <param name="rangeFactory">Delegate that returns a range of values.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rangeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsNotInRangeConstraint<TMessage, TValue>(Func<TMessage, IRange<TValue>> rangeFactory, string errorMessage = null)
        {
            if (rangeFactory == null)
            {
                throw new ArgumentNullException("rangeFactory");
            }
            return New.Constraint<TMessage, TValue>((member, message) => !rangeFactory.Invoke(message).Contains(member))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotInRange)
                .WithErrorMessageArguments(message => new { Range = rangeFactory.Invoke(message) })
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
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint<TMessage, TValue>(left, right, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint<TMessage, TValue>(left, right, options, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint<TMessage, TValue>(left, right, comparer, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint<TMessage, TValue>(left, right, comparer, options, errorMessage));
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
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IRange<TValue> range, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint<TMessage, TValue>(range, errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces 
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(leftFactory, rightFactory, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(leftFactory, rightFactory, options, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(leftFactory, rightFactory, comparer, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(leftFactory, rightFactory, comparer, options, errorMessage));            
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified <paramref name="rangeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="rangeFactory">Delegate that returns a range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="rangeFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IRange<TValue>> rangeFactory, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInRangeConstraint(rangeFactory, errorMessage));            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(TValue left, TValue right, string errorMessage = null)
        {
            return IsInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            return IsInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right, null, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right, comparer), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="left"/> and/or <paramref name="right"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            return IsInRangeConstraint<TMessage, TValue>(new RangeAdapter<TValue>(left, right, comparer, options), errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(IRange<TValue> range, string errorMessage = null)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            return New.Constraint<TMessage, TValue>(range.Contains)
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsInRange)
                .WithErrorMessageArguments(new { Range = range })
                .BuildConstraint();           
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>  
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            return IsInRangeConstraint(leftFactory, rightFactory, null, RangeOptions.None, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            return IsInRangeConstraint(leftFactory, rightFactory, null, options, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            return IsInRangeConstraint(leftFactory, rightFactory, comparer, RangeOptions.None, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="options">
        /// The options that define whether or not <paramref name="leftFactory"/> and/or <paramref name="rightFactory"/> are part of the range themselves.
        /// </param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return IsInRangeConstraint<TMessage, TValue>(message => new RangeAdapter<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), comparer, options), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is within a certain range.
        /// </summary>        
        /// <param name="rangeFactory">Delegate that returns a range of values.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rangeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, TValue, TValue> IsInRangeConstraint<TMessage, TValue>(Func<TMessage, IRange<TValue>> rangeFactory, string errorMessage = null)
        {
            if (rangeFactory == null)
            {
                throw new ArgumentNullException("rangeFactory");
            }
            return New.Constraint<TMessage, TValue>((member, message) => rangeFactory.Invoke(message).Contains(member))
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsInRange)
                .WithErrorMessageArguments(message => new { Range = rangeFactory.Invoke(message) })
                .BuildConstraint();           
        }       

        #endregion        
    }
}
