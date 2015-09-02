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
        public static IMemberConstraint<T, string> IsNotNullOrEmpty<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                        
            return member.Satisfies(IsNotNullOrEmptyConstraint(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string is not null or empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> IsNotNullOrEmptyConstraint(string errorMessage = null)
        {
            return New.Constraint<string>(value => !string.IsNullOrEmpty(value))
                .WithDisplayFormat("{member.Name} != null && {member.Name}.Length > 0")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_NotNullOrEmpty)
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
        public static IMemberConstraint<T, string> IsNullOrEmpty<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullOrEmptyConstraint(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string is null or empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> IsNullOrEmptyConstraint(string errorMessage = null)
        {
            return New.Constraint<string>(string.IsNullOrEmpty)
                .WithDisplayFormat("{member.Name} == null || {member.Name}.Length == 0")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_NullOrEmpty)
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
        public static IMemberConstraint<T, string> IsNotNullOrWhiteSpace<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                 
            return member.Satisfies(IsNotNullOrWhiteSpaceConstraint(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string is not null or white space.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> IsNotNullOrWhiteSpaceConstraint(string errorMessage = null)
        {
            return New.Constraint<string>(value => !string.IsNullOrWhiteSpace(value))
                .WithDisplayFormat("!string.IsNullOrWhiteSpace({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_NotNullOrWhiteSpace)
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
        public static IMemberConstraint<T, string> IsNullOrWhiteSpace<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullOrWhiteSpaceConstraint(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string is null or white space.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> IsNullOrWhiteSpaceConstraint(string errorMessage = null)
        {
            return New.Constraint<string>(string.IsNullOrWhiteSpace)
                .WithDisplayFormat("string.IsNullOrWhiteSpace({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_NullOrWhiteSpace)
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
        public static IMemberConstraint<T, string> IsNotEqualTo<T>(this IMemberConstraint<T, string> member, string other, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsNotEqualToConstraint(other, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string is not equal to another string.
        /// </summary>        
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> IsNotEqualToConstraint(string other, StringComparison compareType, string errorMessage = null)
        {
            return New.Constraint<string>(value => !string.Equals(value, other, compareType))
                .WithDisplayFormat("{member.Name} != {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsNotEqualTo)
                .WithArguments(new { Other = other })
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
        public static IMemberConstraint<T, string> IsEqualTo<T>(this IMemberConstraint<T, string> member, string other, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsEqualToConstraint(other, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string is equal to another string.
        /// </summary>        
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> IsEqualToConstraint(string other, StringComparison compareType, string errorMessage = null)
        {
            return New.Constraint<string>(value => string.Equals(value, other, compareType))
                .WithDisplayFormat("{member.Name} == {member.Other}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.MemberConstraints_IsEqualTo)
                .WithArguments(new { Other = other })
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
        public static IMemberConstraint<T, string> DoesNotStartWith<T>(this IMemberConstraint<T, string> member, string prefix, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotStartWithConstraint(prefix, errorMessage));
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
        public static IMemberConstraint<T, string> DoesNotStartWith<T>(this IMemberConstraint<T, string> member, string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotStartWithConstraint(prefix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not start with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should not start with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotStartWithConstraint(string prefix, string errorMessage = null)
        {
            return DoesNotStartWithConstraint(prefix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not start with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should not start with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotStartWithConstraint(string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            return New.Constraint<string>(value => !value.StartsWith(prefix, compareType))
                .WithDisplayFormat("!{member.Name}.StartsWith({member.Prefix})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotStartWith)
                .WithArguments(new { Prefix = prefix })
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
        public static IMemberConstraint<T, string> StartsWith<T>(this IMemberConstraint<T, string> member, string prefix, string errorMessage = null)
        {            
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(StartsWithConstraint(prefix, errorMessage));
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
        public static IMemberConstraint<T, string> StartsWith<T>(this IMemberConstraint<T, string> member, string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(StartsWithConstraint(prefix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string starts with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should start with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> StartsWithConstraint(string prefix, string errorMessage = null)
        {             
            return StartsWithConstraint(prefix, StringComparison.Ordinal, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string starts with a certain value.
        /// </summary>   
        /// <param name="prefix">The prefix this value should start with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> StartsWithConstraint(string prefix, StringComparison compareType, string errorMessage = null)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            return New.Constraint<string>(value => value.StartsWith(prefix, compareType))
                .WithDisplayFormat("{member.Name}.StartsWith({member.Prefix})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_StartsWith)
                .WithArguments(new { Prefix = prefix })
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
        public static IMemberConstraint<T, string> DoesNotEndWith<T>(this IMemberConstraint<T, string> member, string postfix, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotEndWithConstraint(postfix, errorMessage));
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
        public static IMemberConstraint<T, string> DoesNotEndWith<T>(this IMemberConstraint<T, string> member, string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotEndWithConstraint(postfix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not end with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should not end with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotEndWithConstraint(string postfix, string errorMessage = null)
        {
            return DoesNotEndWithConstraint(postfix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not end with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should not end with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotEndWithConstraint(string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (postfix == null)
            {
                throw new ArgumentNullException("postfix");
            }
            return New.Constraint<string>(value => !value.EndsWith(postfix, compareType))
                .WithDisplayFormat("!{member.Name}.EndsWith({member.Postfix})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotEndWith)
                .WithArguments(new { Postfix = postfix })
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
        public static IMemberConstraint<T, string> EndsWith<T>(this IMemberConstraint<T, string> member, string postfix, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(EndsWithConstraint(postfix, errorMessage));
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
        public static IMemberConstraint<T, string> EndsWith<T>(this IMemberConstraint<T, string> member, string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                     
            return member.Satisfies(EndsWithConstraint(postfix, compareType, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string ends with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should end with.</param>     
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> EndsWithConstraint(string postfix, string errorMessage = null)
        {
            return EndsWithConstraint(postfix, StringComparison.Ordinal, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string ends with a certain value.
        /// </summary>   
        /// <param name="postfix">The postfix this value should start with.</param> 
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>    
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> EndsWithConstraint(string postfix, StringComparison compareType, string errorMessage = null)
        {
            if (postfix == null)
            {
                throw new ArgumentNullException("postfix");
            }
            return New.Constraint<string>(value => value.EndsWith(postfix, compareType))
                .WithDisplayFormat("{member.Name}.EndsWith({member.Postfix})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_EndsWith)
                .WithArguments(new { Postfix = postfix })
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
        public static IMemberConstraint<T, string> DoesNotContain<T>(this IMemberConstraint<T, string> member, char value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotContainConstraint(value, errorMessage));
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
        public static IMemberConstraint<T, string> DoesNotContain<T>(this IMemberConstraint<T, string> member, string value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                                   
            return member.Satisfies(DoesNotContainConstraint(value, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not contain a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotContainConstraint(char value, string errorMessage = null)
        {
            return DoesNotContainConstraint(value.ToString(CultureInfo.CurrentCulture), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not contain a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value "/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotContainConstraint(string value, string errorMessage = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return New.Constraint<string>(v => !v.Contains(value))
                .WithDisplayFormat("!{member.Name}.Contains({member.Value})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotContain)
                .WithArguments(new { Value = value })
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
        public static IMemberConstraint<T, string> Contains<T>(this IMemberConstraint<T, string> member, char value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(ContainsConstraint(value, errorMessage));
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
        public static IMemberConstraint<T, string> Contains<T>(this IMemberConstraint<T, string> member, string value, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                       
            return member.Satisfies(ContainsConstraint(value, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string contains a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> ContainsConstraint(char value, string errorMessage = null)
        {
            return ContainsConstraint(value.ToString(CultureInfo.CurrentCulture), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string contains a certain value.
        /// </summary>        
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value "/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> ContainsConstraint(string value, string errorMessage = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return New.Constraint<string>(v => v.Contains(value))
                .WithDisplayFormat("{member.Name}.Contains({member.Value})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_Contains)
                .WithArguments(new { Value = value })
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
        public static IMemberConstraint<T, string> DoesNotMatch<T>(this IMemberConstraint<T, string> member, string pattern, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(DoesNotMatchConstraint(pattern, errorMessage));
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
        public static IMemberConstraint<T, string> DoesNotMatch<T>(this IMemberConstraint<T, string> member, string pattern, RegexOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                       
            return member.Satisfies(DoesNotMatchConstraint(pattern, options, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not match a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotMatchConstraint(string pattern, string errorMessage = null)
        {
            return DoesNotMatchConstraint(pattern, RegexOptions.None, errorMessage);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string does not match a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> DoesNotMatchConstraint(string pattern, RegexOptions options, string errorMessage = null)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return New.Constraint<string>(value => !Regex.IsMatch(value, pattern, options))
                .WithDisplayFormat("!{member.Name}.Matches({member.Pattern})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_DoesNotMatch)
                .WithArguments(new { Pattern = pattern })
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
        public static IMemberConstraint<T, string> Matches<T>(this IMemberConstraint<T, string> member, string pattern, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(MatchesConstraint(pattern, errorMessage));
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
        public static IMemberConstraint<T, string> Matches<T>(this IMemberConstraint<T, string> member, string pattern, RegexOptions options, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                      
            return member.Satisfies(MatchesConstraint(pattern, options, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string matches a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> MatchesConstraint(string pattern, string errorMessage = null)
        {
            return MatchesConstraint(pattern, RegexOptions.None, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string matches a certain pattern.
        /// </summary>        
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> MatchesConstraint(string pattern, RegexOptions options, string errorMessage = null)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return New.Constraint<string>(value => Regex.IsMatch(value, pattern, options))
                .WithDisplayFormat("{member.Name}.Matches({member.Pattern})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_Matches)
                .WithArguments(new { Pattern = pattern })
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
        public static IMemberConstraint<T, string> HasLengthOf<T>(this IMemberConstraint<T, string> member, int length, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(HasLengthOfConstraint(length, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string has a specific length.
        /// </summary>        
        /// <param name="length">The required length of the string.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>        
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is smaller than <c>0</c>.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> HasLengthOfConstraint(int length, string errorMessage = null)
        {
            if (length < 0)
            {
                throw NewNegativeLengthException("length", length);
            }
            return New.Constraint<string>(value => value.Length == length)
                .WithDisplayFormat("{member.Name}.Length == {member.Length}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_HasLengthOf)
                .WithArguments(new { Length = length })
                .BuildConstraint();
        }

        #endregion

        #region [====== HasLengthBetween ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="minLength">The minimum length of the string (inclusive).</param>
        /// <param name="maxLength">The maximum length of the string (inclusive).</param>
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
        /// Either <paramref name="minLength"/> or <paramref name="maxLength "/> is smaller than 0, or
        /// <paramref name="maxLength"/> is smaller than <paramref name="minLength"/>.
        /// </exception>
        public static IMemberConstraint<T, string> HasLengthBetween<T>(this IMemberConstraint<T, string> member, int minLength, int maxLength, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(HasLengthBetweenConstraint(minLength, maxLength, errorMessage));
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
        public static IMemberConstraint<T, string> HasLengthBetween<T>(this IMemberConstraint<T, string> member, IRange<int> range, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                       
            return member.Satisfies(HasLengthBetweenConstraint(range, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string has a length within a specific range.
        /// </summary>        
        /// <param name="left">The minimum length of the string (inclusive).</param>
        /// <param name="right">The maximum length of the string (inclusive).</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="left"/> or <paramref name="right"/> is smaller than 0, or
        /// <paramref name="right"/> is smaller than <paramref name="left"/>.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> HasLengthBetweenConstraint(int left, int right, string errorMessage = null)
        {
            if (left < 0)
            {
                throw NewNegativeLengthException("left", left);
            }
            if (right < 0)
            {
                throw NewNegativeLengthException("right", right);
            }
            return HasLengthBetweenConstraint(new RangeAdapter<int>(left, right), errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string has a length within a specific range.
        /// </summary>        
        /// <param name="range">A range of allowable lengths.</param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, string> HasLengthBetweenConstraint(IRange<int> range, string errorMessage = null)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            return New.Constraint<string>(value => range.Contains(value.Length))
                .WithDisplayFormat("{member.Name}.Length in {member.Range}")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_HasLengthBetween)
                .WithArguments(new { Range = range })
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
        public static IMemberConstraint<T, byte> IsByte<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, byte> IsByte<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsByteConstraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a byte.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, byte> IsByteConstraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {            
            return New.Constraint(delegate(string value, out byte result) { return byte.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("byte.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsByte)
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
        public static IMemberConstraint<T, sbyte> IsSByte<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, sbyte> IsSByte<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSByteConstraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a signed byte.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, sbyte> IsSByteConstraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return New.Constraint(delegate(string value, out sbyte result) { return sbyte.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("sbyte.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsSByte)
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
        public static IMemberConstraint<T, char> IsChar<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsCharConstraint(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a byte.
        /// </summary>                
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, char> IsCharConstraint(string errorMessage = null)
        {
            return New.Constraint<string, char>(value => value != null && value.Length == 1, value => value[0])
                .WithDisplayFormat("{member.Name}[0]")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsChar)
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
        public static IMemberConstraint<T, short> IsInt16<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, short> IsInt16<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInt16Constraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a 16-bit integer.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, short> IsInt16Constraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return New.Constraint(delegate(string value, out short result) { return short.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("short.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsInt16)
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
        public static IMemberConstraint<T, int> IsInt32<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, int> IsInt32<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInt32Constraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a 32-bit integer.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, int> IsInt32Constraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return New.Constraint(delegate(string value, out int result) { return int.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("int.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsInt32)
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
        public static IMemberConstraint<T, long> IsInt64<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, long> IsInt64<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsInt64Constraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a 64-bit integer.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, long> IsInt64Constraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return New.Constraint(delegate(string value, out long result) { return long.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("long.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsInt64)
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
        public static IMemberConstraint<T, float> IsSingle<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, float> IsSingle<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsSingleConstraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a 32-bit floating point number.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, float> IsSingleConstraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return New.Constraint(delegate(string value, out float result) { return float.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("float.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsSingle)
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
        public static IMemberConstraint<T, double> IsDouble<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, double> IsDouble<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsDoubleConstraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a 64-bit floating point number.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, double> IsDoubleConstraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return New.Constraint(delegate(string value, out double result) { return double.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("double.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsDouble)
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
        public static IMemberConstraint<T, decimal> IsDecimal<T>(this IMemberConstraint<T, string> member, string errorMessage = null)
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
        public static IMemberConstraint<T, decimal> IsDecimal<T>(this IMemberConstraint<T, string> member, NumberStyles style, IFormatProvider formatProvider, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsDecimalConstraint(style, formatProvider, errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a string can be converted to a 96-bit floating point number.
        /// </summary>        
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in a value.
        /// </param>
        /// <param name="formatProvider">
        /// An object that supplies culture-specific formatting information about a value.
        /// </param>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<string, decimal> IsDecimalConstraint(NumberStyles style, IFormatProvider formatProvider = null, string errorMessage = null)
        {
            return New.Constraint(delegate(string value, out decimal result) { return decimal.TryParse(value, style, formatProvider, out result); })
                .WithDisplayFormat("decimal.Parse({member.Name})")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.StringConstraints_IsDecimal)
                .BuildConstraint();
        }

        #endregion        
    }
}
