using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, object> otherFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>     
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is not equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>           
        public static IMemberConstraint<TMessage, TValue> IsNotEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IEquatable<TValue>> otherFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== IsEqualTo ======]

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="other">The instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>   
        /// <returns>A member that has been merged with the specified member.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>          
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, object> otherFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception> 
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>
        /// <param name="comparer">The comparer that is used to perform the comparison.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>        
        /// <returns>A member that has been merged with the specified member.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="otherFactory"/> is <c>null</c>.
        /// </exception>    
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, TValue> otherFactory, IEqualityComparer<TValue> comparer, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the member's value is equal to <paramref name="otherFactory"/>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="otherFactory">Delegate that returns the instance to compare the member's value to.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param> 
        /// <returns>A member that has been merged with the specified member.</returns>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>    
        public static IMemberConstraint<TMessage, TValue> IsEqualTo<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, Func<TMessage, IEquatable<TValue>> otherFactory, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
