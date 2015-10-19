using System;
using System.Collections.Generic;

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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            throw new NotImplementedException();           
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();            
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            throw new NotImplementedException();           
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();           
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
            throw new NotImplementedException();            
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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces 
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="left"/> and <paramref name="right"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, TValue left, TValue right, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="member"/> or <paramref name="range"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, IRange<TValue> range, string errorMessage = null)
        {
            throw new NotImplementedException();
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces 
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, string errorMessage = null)
        {
            throw new NotImplementedException();           
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or neither of these values
        /// implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();            
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, string errorMessage = null)
        {
            throw new NotImplementedException();            
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
        /// <paramref name="leftFactory"/> and <paramref name="rightFactory"/> do not represent a valid range,, or, if the default <paramref name="comparer"/>
        /// is used, neither of these values implement the <see cref="IComparable{TValue}" /> or <see cref="IComparable"/> interfaces.
        /// - or -        
        /// <paramref name="errorMessage"/> is not in a correct format.        
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> leftFactory, Func<TMessage, TValue> rightFactory, IComparer<TValue> comparer, RangeOptions options, string errorMessage = null)
        {
            throw new NotImplementedException();            
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
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        public static IMemberConstraint<TMessage, TValue> IsInRange<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IRange<TValue>> rangeFactory, string errorMessage = null)
        {
            throw new NotImplementedException();           
        }          

        #endregion        
    }
}
