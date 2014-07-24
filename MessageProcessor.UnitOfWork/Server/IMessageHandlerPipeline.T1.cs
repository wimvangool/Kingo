using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Server
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="IMessageHandler{T}" /> that provides
    /// access to the attributes declared on it.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle.</typeparam>
    public interface IMessageHandlerPipeline<in TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        #region [====== Class Attributes ======]

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that was declared on the class of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class;        

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that was declared on the class of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetClassAttributeOfType<TAttribute>(bool includeInherited, out TAttribute attribute) where TAttribute : class;        

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that was declared on the class of this handler.
        /// </summary>
        /// <param name="type">Type of the attribute to retrieve.</param>        
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetClassAttributeOfType(Type type, out object attribute);        

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that is declared on the class of this handler.
        /// </summary>
        /// <param name="type">Type of the attribute to retrieve.</param>
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>       
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetClassAttributeOfType(Type type, bool includeInherited, out object attribute);        

        /// <summary>
        /// Returns all attributes of the specified type that are declared on the class of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attributes to retrieve.</typeparam>
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>
        /// <returns>All attributes of the specified type.</returns>
        IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>(bool includeInherited = false);        

        /// <summary>
        /// Returns all attributes of the specified type that are declared on the class of this handler.
        /// </summary>
        /// <param name="type">Type of the attribute to retrieve.</param>        
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>
        /// <returns>All attributes of the specified type.</returns>
        IEnumerable<object> GetClassAttributesOfType(Type type, bool includeInherited = false);        

        #endregion

        #region [====== Method Attributes ======]

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that was declared on the method of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class;        

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that was declared on the method of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to retrieve.</typeparam>
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetMethodAttributeOfType<TAttribute>(bool includeInherited, out TAttribute attribute) where TAttribute : class;        

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that was declared on the method of this handler.
        /// </summary>
        /// <param name="type">Type of the attribute to retrieve.</param>        
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetMethodAttributeOfType(Type type, out object attribute);        

        /// <summary>
        /// Attempts to retrieve the attribute of the specified type that is declared on the method of this handler.
        /// </summary>
        /// <param name="type">Type of the attribute to retrieve.</param>
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>       
        /// <param name="attribute">
        /// When this method has returned, contains the retrieved attribute, if it was found, or <c>null</c>
        /// if it was not found.
        /// </param>        
        /// <returns><c>true</c> if the attribute was found; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Multiple attributes of the specified type were found.
        /// </exception>
        bool TryGetMethodAttributeOfType(Type type, bool includeInherited, out object attribute);        

        /// <summary>
        /// Returns all attributes of the specified type that are declared on the method of this handler.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attributes to retrieve.</typeparam>
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>
        /// <returns>All attributes of the specified type.</returns>
        IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>(bool includeInherited = false);

        /// <summary>
        /// Returns all attributes of the specified type that are declared on the method of this handler.
        /// </summary>
        /// <param name="type">Type of the attribute to retrieve.</param>        
        /// <param name="includeInherited">
        /// Indicates whether or not the inheritance tree must be searched for the attribute.
        /// </param>
        /// <returns>All attributes of the specified type.</returns>
        IEnumerable<object> GetMethodAttributesOfType(Type type, bool includeInherited = false);        

        #endregion
    }
}
