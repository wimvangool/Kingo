using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{String}" />.
    /// </summary>
    public static class StringConstraints
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> IsNotNullOrEmpty<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                        
            return member.Satisfies(IsNotNullOrEmptyConstraint<TMessage>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string is not null or empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> IsNotNullOrEmptyConstraint<TMessage>(string errorMessage = null)
        {
            return New.Constraint<TMessage, string>(member => !string.IsNullOrEmpty(member))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_NotNullOrEmpty)
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsNullOrEmpty ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> IsNullOrEmpty<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullOrEmptyConstraint<TMessage>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string is null or empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> IsNullOrEmptyConstraint<TMessage>(string errorMessage = null)
        {
            return New.Constraint<TMessage, string>(member => string.IsNullOrEmpty(member))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_NullOrEmpty)
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsNotNullOrWhiteSpace ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> IsNotNullOrWhiteSpace<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                 
            return member.Satisfies(IsNotNullOrWhiteSpaceConstraint<TMessage>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string is not null or white space.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> IsNotNullOrWhiteSpaceConstraint<TMessage>(string errorMessage = null)
        {
            return New.Constraint<TMessage, string>(member => !string.IsNullOrWhiteSpace(member))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_NotNullOrWhiteSpace)
                .BuildConstraint();
        }        

        #endregion        

        #region [====== IsNullOrWhiteSpace ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> IsNullOrWhiteSpace<TMessage>(this IMemberConstraint<TMessage, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullOrWhiteSpaceConstraint<TMessage>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string is null or white space.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> IsNullOrWhiteSpaceConstraint<TMessage>(string errorMessage = null)
        {
            return New.Constraint<TMessage, string>(member => string.IsNullOrWhiteSpace(member))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_NullOrWhiteSpace)
                .BuildConstraint();
        }       

        #endregion        

        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> IsNotEqualTo<TMessage>(this IMemberConstraint<TMessage, string> member, string other, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsNotEqualToConstraint<TMessage>(other, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string is not equal to another string.
        /// </summary>        
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> IsNotEqualToConstraint<TMessage>(string other, StringComparison compareType, string errorMessage = null)
        {
            return New.Constraint<TMessage, string>(member => !string.Equals(member, other, compareType))              
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsEqualTo ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> IsEqualTo<TMessage>(this IMemberConstraint<TMessage, string> member, string other, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsEqualToConstraint<TMessage>(other, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string is equal to another string.
        /// </summary>        
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> IsEqualToConstraint<TMessage>(string other, StringComparison compareType, string errorMessage = null)
        {
            return New.Constraint<TMessage, string>(member => string.Equals(member, other, compareType))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithErrorMessageArguments(new { Other = other })
                .BuildConstraint();
        }        

        #endregion

        #region [====== DoesNotStartWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not start with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should not start with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotStartWith<TMessage>(this IMemberConstraint<TMessage, string> member, string prefix, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotStartWithConstraint<TMessage>(prefix, errorMessage));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not start with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should not start with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotStartWith<TMessage>(this IMemberConstraint<TMessage, string> member, string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotStartWithConstraint<TMessage>(prefix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not start with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should not start with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotStartWithConstraint<TMessage>(string prefix, string errorMessage = null)
        {
            return DoesNotStartWithConstraint<TMessage>(prefix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not start with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should not start with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotStartWithConstraint<TMessage>(string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            return New.Constraint<TMessage, string>(member => !member.StartsWith(prefix, compareType))              
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotStartWith)
                .WithErrorMessageArguments(new { Prefix = prefix })
                .BuildConstraint();
        }

        #endregion

        #region [====== StartsWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> StartsWith<TMessage>(this IMemberConstraint<TMessage, string> member, string prefix, string errorMessage = null)
        {            
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(StartsWithConstraint<TMessage>(prefix, errorMessage));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> StartsWith<TMessage>(this IMemberConstraint<TMessage, string> member, string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(StartsWithConstraint<TMessage>(prefix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string starts with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should start with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> StartsWithConstraint<TMessage>(string prefix, string errorMessage = null)
        {             
            return StartsWithConstraint<TMessage>(prefix, StringComparison.Ordinal, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string starts with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should start with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> StartsWithConstraint<TMessage>(string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            return New.Constraint<TMessage, string>(member => member.StartsWith(prefix, compareType))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_StartsWith)
                .WithErrorMessageArguments(new { Prefix = prefix })
                .BuildConstraint();
        }                

        #endregion

        #region [====== DoesNotEndWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not end with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should not end with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotEndWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotEndWithConstraint<TMessage>(postfix, errorMessage));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not end with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should not end with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotEndWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotEndWithConstraint<TMessage>(postfix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not end with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should not end with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotEndWithConstraint<TMessage>(string postfix, string errorMessage = null)
        {
            return DoesNotEndWithConstraint<TMessage>(postfix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not end with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should not end with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotEndWithConstraint<TMessage>(string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (postfix == null)
            {
                throw new ArgumentNullException("postfix");
            }
            return New.Constraint<TMessage, string>(member => !member.EndsWith(postfix, compareType))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotEndWith)
                .WithErrorMessageArguments(new { Postfix = postfix })
                .BuildConstraint();
        }

        #endregion

        #region [====== EndsWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> EndsWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(EndsWithConstraint<TMessage>(postfix, errorMessage));
        }        

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> EndsWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                     
            return member.Satisfies(EndsWithConstraint<TMessage>(postfix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string ends with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should end with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> EndsWithConstraint<TMessage>(string postfix, string errorMessage = null)
        {
            return EndsWithConstraint<TMessage>(postfix, StringComparison.Ordinal, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string ends with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should start with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> EndsWithConstraint<TMessage>(string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (postfix == null)
            {
                throw new ArgumentNullException("postfix");
            }
            return New.Constraint<TMessage, string>(member => member.EndsWith(postfix, compareType))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_EndsWith)
                .WithErrorMessageArguments(new { Postfix = postfix })
                .BuildConstraint();
        }         

        #endregion

        #region [====== DoesNotContain ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotContain<TMessage>(this IMemberConstraint<TMessage, string> member, char value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotContainConstraint<TMessage>(value, errorMessage));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotContain<TMessage>(this IMemberConstraint<TMessage, string> member, string value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                                   
            return member.Satisfies(DoesNotContainConstraint<TMessage>(value, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not contain a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotContainConstraint<TMessage>(char value, string errorMessage = null)
        {
            return DoesNotContainConstraint<TMessage>(value.ToString(CultureInfo.CurrentCulture), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not contain a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value "/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotContainConstraint<TMessage>(string value, string errorMessage = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return New.Constraint<TMessage, string>(member => !member.Contains(value))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotContain)
                .WithErrorMessageArguments(new { Value = value })
                .BuildConstraint();
        }        

        #endregion

        #region [====== Contains ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> Contains<TMessage>(this IMemberConstraint<TMessage, string> member, char value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(ContainsConstraint<TMessage>(value, errorMessage));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> Contains<TMessage>(this IMemberConstraint<TMessage, string> member, string value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                       
            return member.Satisfies(ContainsConstraint<TMessage>(value, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string contains a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> ContainsConstraint<TMessage>(char value, string errorMessage = null)
        {
            return ContainsConstraint<TMessage>(value.ToString(CultureInfo.CurrentCulture), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string contains a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value "/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> ContainsConstraint<TMessage>(string value, string errorMessage = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return New.Constraint<TMessage, string>(member => member.Contains(value))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_Contains)
                .WithErrorMessageArguments(new { Value = value })
                .BuildConstraint();
        }        

        #endregion

        #region [====== DoesNotMatch ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotMatch<TMessage>(this IMemberConstraint<TMessage, string> member, string pattern, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotMatchConstraint<TMessage>(pattern, errorMessage));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotMatch<TMessage>(this IMemberConstraint<TMessage, string> member, string pattern, RegexOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                       
            return member.Satisfies(DoesNotMatchConstraint<TMessage>(pattern, options, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not match a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotMatchConstraint<TMessage>(string pattern, string errorMessage = null)
        {
            return DoesNotMatchConstraint<TMessage>(pattern, RegexOptions.None, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string does not match a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> DoesNotMatchConstraint<TMessage>(string pattern, RegexOptions options, string errorMessage = null)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return New.Constraint<TMessage, string>(member => !Regex.IsMatch(member, pattern, options))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotMatch)
                .WithErrorMessageArguments(new { Pattern = pattern })
                .BuildConstraint();
        }        

        #endregion

        #region [====== Matches ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> Matches<TMessage>(this IMemberConstraint<TMessage, string> member, string pattern, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(MatchesConstraint<TMessage>(pattern, errorMessage));
        }                

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> Matches<TMessage>(this IMemberConstraint<TMessage, string> member, string pattern, RegexOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                      
            return member.Satisfies(MatchesConstraint<TMessage>(pattern, options, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string matches a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> MatchesConstraint<TMessage>(string pattern, string errorMessage = null)
        {
            return MatchesConstraint<TMessage>(pattern, RegexOptions.None, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string matches a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration members that provide options for matching.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> MatchesConstraint<TMessage>(string pattern, RegexOptions options, string errorMessage = null)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return New.Constraint<TMessage, string>(member => Regex.IsMatch(member, pattern, options))              
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_Matches)
                .WithErrorMessageArguments(new { Pattern = pattern })
                .BuildConstraint();
        }        

        #endregion        

        #region [====== HasLengthOf ======]
      
        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is equal to <paramref name="length"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="length">The required length of the string.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is smaller than <c>0</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, string> HasLengthOf<TMessage>(this IMemberConstraint<TMessage, string> member, int length, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(HasLengthOfConstraint<TMessage>(length, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string has a specific length.
        /// </summary>        
        /// <param name="length">The required length of the string.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>        
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is smaller than <c>0</c>.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> HasLengthOfConstraint<TMessage>(int length, string errorMessage = null)
        {
            if (length < 0)
            {
                throw NewNegativeLengthException("length", length);
            }
            return New.Constraint<TMessage, string>(member => member.Length == length)               
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_HasLengthOf)
                .WithErrorMessageArguments(new { Length = length })
                .BuildConstraint();
        }

        #endregion

        #region [====== HasLengthBetween ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is between <paramref name="left"/> and <paramref name="right"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="left">The minimum length of the string (inclusive).</param>
        /// <param name="right">The maximum length of the string (inclusive).</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="left"/> or <paramref name="right"/> is smaller than 0, or
        /// <paramref name="right"/> is smaller than <paramref name="left"/>.
        /// </exception>
        public static IMemberConstraint<TMessage, string> HasLengthBetween<TMessage>(this IMemberConstraint<TMessage, string> member, int left, int right, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(HasLengthBetweenConstraint<TMessage>(left, right, errorMessage));
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is in the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="range">A range of allowable lengths.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="range" /> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> HasLengthBetween<TMessage>(this IMemberConstraint<TMessage, string> member, IRange<int> range, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                       
            return member.Satisfies(HasLengthBetweenConstraint<TMessage>(range, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string has a length within a specific range.
        /// </summary>        
        /// <param name="left">The minimum length of the string (inclusive).</param>
        /// <param name="right">The maximum length of the string (inclusive).</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="left"/> or <paramref name="right"/> is smaller than 0, or
        /// <paramref name="right"/> is smaller than <paramref name="left"/>.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> HasLengthBetweenConstraint<TMessage>(int left, int right, string errorMessage = null)
        {
            if (left < 0)
            {
                throw NewNegativeLengthException("left", left);
            }
            if (right < 0)
            {
                throw NewNegativeLengthException("right", right);
            }
            return HasLengthBetweenConstraint<TMessage>(new RangeAdapter<int>(left, right), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string has a length within a specific range.
        /// </summary>        
        /// <param name="range">A range of allowable lengths.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, string> HasLengthBetweenConstraint<TMessage>(IRange<int> range, string errorMessage = null)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            return New.Constraint<TMessage, string>(member => range.Contains(member.Length))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_HasLengthBetween)
                .WithErrorMessageArguments(new { Range = range })
                .BuildConstraint();
        }        

        private static Exception NewNegativeLengthException(string paramName, int length)
        {
            var messageFormat = ExceptionMessages.StringMemberExtensions_NegativeLength;
            var message = string.Format(messageFormat, length);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #endregion

        #region [====== IsByte ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="byte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsByte(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsByteConstraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a byte.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, byte> IsByteConstraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, byte>.Implementation implementation = delegate(string member, TMessage message, out byte result)
            {
                return byte.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation)               
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsByte)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsSByte ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsSByte(member, NumberStyles.Integer | NumberStyles.AllowTrailingSign, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSByteConstraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a signed byte.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, sbyte> IsSByteConstraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, sbyte>.Implementation implementation = delegate(string member, TMessage message, out sbyte result)
            {
                return sbyte.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation) 
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsSByte)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsChar ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="char"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsCharConstraint<TMessage>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a byte.
        /// </summary>                
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, char> IsCharConstraint<TMessage>(string errorMessage = null)
        {
            return New.Constraint<TMessage, string, char>(member => member != null && member.Length == 1, member => member[0])                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsChar)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsInt16 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="short"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsInt16(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInt16Constraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a 16-bit integer.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, short> IsInt16Constraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, short>.Implementation implementation = delegate(string member, TMessage message, out short result)
            {
                return short.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation) 
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsInt16)
                .BuildConstraint();
        }

        #endregion        

        #region [====== IsInt32 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="int"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsInt32(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInt32Constraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a 32-bit integer.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration members that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, int> IsInt32Constraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, int>.Implementation implementation = delegate(string member, TMessage message, out int result)
            {
                return int.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation) 
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsInt32)
                .BuildConstraint();
        }

        #endregion        

        #region [====== IsInt64 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="long"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsInt64(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInt64Constraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a 64-bit integer.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, long> IsInt64Constraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, long>.Implementation implementation = delegate(string member, TMessage message, out long result)
            {
                return long.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation) 
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsInt64)
                .BuildConstraint();
        }

        #endregion      

        #region [====== IsSingle ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="float"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsSingle(member, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSingleConstraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a 32-bit floating point number.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, float> IsSingleConstraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, float>.Implementation implementation = delegate(string member, TMessage message, out float result)
            {
                return float.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation) 
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsSingle)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsDouble ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="double"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsDouble(member, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsDoubleConstraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a 64-bit floating point number.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, double> IsDoubleConstraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, double>.Implementation implementation = delegate(string member, TMessage message, out double result)
            {
                return double.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation) 
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsDouble)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsDecimal ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="decimal"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            return IsDecimal(member, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, errorMessage);
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
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
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
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsDecimalConstraint<TMessage>(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a string can be converted to a 96-bit floating point number.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, string, decimal> IsDecimalConstraint<TMessage>(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            DelegateConstraint<TMessage, string, decimal>.Implementation implementation = delegate(string member, TMessage message, out decimal result)
            {
                return decimal.TryParse(member, style, formatProvider, out result);
            };
            return New.Constraint(implementation) 
                .WithErrorMessage(errorMessage ?? ConstraintErrors.StringConstraints_IsDecimal)
                .BuildConstraint();
        }

        #endregion        
    }
}
