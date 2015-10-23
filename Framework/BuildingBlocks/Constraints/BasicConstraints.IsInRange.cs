using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class BasicConstraints
    {                                
        #region [====== IsNotInRange ======]

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range        
        /// - or -
        /// the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            return member.Apply(new IsNotInRangeConstraint<TValue>(left, right).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            return member.Apply(new IsNotInRangeConstraint<TValue>(left, right, options).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range        
        /// - or -
        /// <paramref name="comparer"/> is <c>null</c> and the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsNotInRangeConstraint<TValue>(left, right, comparer).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>              
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// <paramref name="comparer"/> is <c>null</c> and the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            return member.Apply(new IsNotInRangeConstraint<TValue>(left, right, comparer, options).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            return member.Apply(new IsNotInRangeConstraint<TValue>(range).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsNotInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message)).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsNotInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), options).WithErrorMessage(errorMessage));           
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsNotInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));          
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>              
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsNotInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), comparer, options).WithErrorMessage(errorMessage));         
        }

        /// <summary>
        /// Verifies that the member's value does not lie within the specified <paramref name="rangeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="rangeFactory">Delegate that returns a range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            if (rangeFactory == null)
            {
                throw new ArgumentNullException("rangeFactory");
            }
            return member.Apply(message => new IsNotInRangeConstraint<TValue>(rangeFactory.Invoke(message)).WithErrorMessage(errorMessage));           
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range        
        /// - or -
        /// the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            return member.Apply(new IsInRangeConstraint<TValue>(left, right).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            return member.Apply(new IsInRangeConstraint<TValue>(left, right, options).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="left">The lower boundary of the range.</param>
        /// <param name="right">The upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range        
        /// - or -
        /// <paramref name="comparer"/> is <c>null</c> and the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            return member.Apply(new IsInRangeConstraint<TValue>(left, right, comparer).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>              
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// <paramref name="comparer"/> is <c>null</c> and the specified instances do not implement the <see cref="IComparable{T}" /> interface
        /// - or -
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            return member.Apply(new IsInRangeConstraint<TValue>(left, right, comparer, options).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="range">A range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IRange<TValue> range, string errorMessage = null)
        {
            return member.Apply(new IsInRangeConstraint<TValue>(range).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>    
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>           
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message)).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>  
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), options).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified range.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="leftFactory">Delegate that returns the lower boundary of the range.</param>
        /// <param name="rightFactory">Delegate that returns the upper boundary of the range.</param>        
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>             
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), comparer).WithErrorMessage(errorMessage));
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
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="leftFactory"/> or <paramref name="rightFactory"/> is <c>null</c>.
        /// </exception>              
        /// <exception cref="ArgumentException">             
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            if (leftFactory == null)
            {
                throw new ArgumentNullException("leftFactory");
            }
            if (rightFactory == null)
            {
                throw new ArgumentNullException("rightFactory");
            }
            return member.Apply(message => new IsInRangeConstraint<TValue>(leftFactory.Invoke(message), rightFactory.Invoke(message), comparer, options).WithErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the member's value lies within the specified <paramref name="rangeFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="rangeFactory">Delegate that returns a range of values.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IRange<TValue>> rangeFactory, string errorMessage = null)
        {
            if (rangeFactory == null)
            {
                throw new ArgumentNullException("rangeFactory");
            }
            return member.Apply(message => new IsInRangeConstraint<TValue>(rangeFactory.Invoke(message)).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== IsNotInRangeConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is within a certain range of values.
    /// </summary>
    public sealed class IsNotInRangeConstraint<TValue> : Constraint<TValue>
    {
        private readonly IRange<TValue> _range;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotInRangeConstraint{T}" /> class.
        /// </summary>
        /// <param name="left">The lower boundary of this range.</param>
        /// <param name="right">The upper boundary of this range.</param>        
        /// <param name="options">
        /// The options indicating whether or <paramref name="left"/> and/or <paramref name="right"/> are part of this range themselves.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// the specified instances do not implement the <see cref="IComparable{T}" /> interface.
        /// </exception>
        public IsNotInRangeConstraint(TValue left, TValue right, RangeOptions options = RangeOptions.None)
            : this(new Range<TValue>(left, right, null, options)) { }  
 
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotInRangeConstraint{T}" /> class.
        /// </summary>
        /// <param name="left">The lower boundary of this range.</param>
        /// <param name="right">The upper boundary of this range.</param>
        /// <param name="comparer">Optional comparer to use when comparing two instances.</param>
        /// <param name="options">
        /// The options indicating whether or <paramref name="left"/> and/or <paramref name="right"/> are part of this range themselves.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// <paramref name="comparer"/> is <c>null</c> and the specified instances do not implement the <see cref="IComparable{T}" /> interface.
        /// </exception>
        public IsNotInRangeConstraint(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options = RangeOptions.None)
            : this(new Range<TValue>(left, right, comparer, options)) { }        

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotInRangeConstraint{T}" /> class.
        /// </summary>   
        /// <param name="range">The range that the value is checked to be a part of.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception> 
        public IsNotInRangeConstraint(IRange<TValue> range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            _range = range;
        }

        private IsNotInRangeConstraint(IsNotInRangeConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _range = constraint._range;
        }

        private IsNotInRangeConstraint(IsNotInRangeConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _range = constraint._range;
        }

        /// <summary>
        /// The range that the value is checked to be a part of.
        /// </summary>
        public IRange<TValue> Range
        {
            get { return _range; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsNotInRange); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsNotInRangeConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsNotInRangeConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsInRangeConstraint<TValue>(_range).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return !_range.Contains(value);
        }

        #endregion
    }

    #endregion

    #region [====== IsInRangeConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is within a certain range of values.
    /// </summary>
    public sealed class IsInRangeConstraint<TValue> : Constraint<TValue>
    {
        private readonly IRange<TValue> _range;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsInRangeConstraint{T}" /> class.
        /// </summary>
        /// <param name="left">The lower boundary of this range.</param>
        /// <param name="right">The upper boundary of this range.</param>        
        /// <param name="options">
        /// The options indicating whether or <paramref name="left"/> and/or <paramref name="right"/> are part of this range themselves.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// the specified instances do not implement the <see cref="IComparable{T}" /> interface.
        /// </exception>
        public IsInRangeConstraint(TValue left, TValue right, RangeOptions options = RangeOptions.None)
            : this(new Range<TValue>(left, right, null, options)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsInRangeConstraint{T}" /> class.
        /// </summary>
        /// <param name="left">The lower boundary of this range.</param>
        /// <param name="right">The upper boundary of this range.</param>
        /// <param name="comparer">Optional comparer to use when comparing two instances.</param>
        /// <param name="options">
        /// The options indicating whether or <paramref name="left"/> and/or <paramref name="right"/> are part of this range themselves.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range
        /// - or -
        /// both are equal and <paramref name="options"/> specifies at least one exclusive boundary
        /// - or -
        /// <paramref name="comparer"/> is <c>null</c> and the specified instances do not implement the <see cref="IComparable{T}" /> interface.
        /// </exception>
        public IsInRangeConstraint(TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options = RangeOptions.None)
            : this(new Range<TValue>(left, right, comparer, options)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsInRangeConstraint{T}" /> class.
        /// </summary>   
        /// <param name="range">The range that the value is checked to be a part of.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="range"/> is <c>null</c>.
        /// </exception> 
        public IsInRangeConstraint(IRange<TValue> range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            _range = range;
        }

        private IsInRangeConstraint(IsInRangeConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _range = constraint._range;
        }

        private IsInRangeConstraint(IsInRangeConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _range = constraint._range;
        }

        /// <summary>
        /// The range that the value is checked to be a part of.
        /// </summary>
        public IRange<TValue> Range
        {
            get { return _range; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsInRange); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new IsInRangeConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new IsInRangeConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNotInRangeConstraint<TValue>(_range).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return _range.Contains(value);
        }

        #endregion
    }

    #endregion
}
